using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Snmp.Business.Abstract;
using Snmp.Business.Abstract.Auth;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.Abstract.Outbox;
using Snmp.Business.Abstract.Redis;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Abstract.Snmp;
using Snmp.Business.Abstract.UserService;
using Snmp.Business.Concrete;
using Snmp.Business.Concrete.Auth;
using Snmp.Business.Concrete.DeviceService;
using Snmp.Business.Concrete.Outbox;
using Snmp.Business.Concrete.Security;
using Snmp.Business.Concrete.SnmpService;
using Snmp.Business.Concrete.UserService;
using Snmp.Business.Mapping;
using Snmp.Business.Queries.Abstract;
using Snmp.Business.Queries.Concrete;
using Snmp.Business.ValidationRules.DeviceValidator;
using Snmp.Common.Configuration;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.DataAccess.Concrete.Redis;
using Snmp.WebAPI.Middlewares;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    var settings = context.Configuration
        .GetSection(ElasticSearchSettings.SectionName)
        .Get<ElasticSearchSettings>()
        ?? throw new InvalidOperationException("ElasticSearch configuration is missing.");

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [CorrelationId:{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(new Uri(settings.Uri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = settings.IndexFormat
            });
});


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>option.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<IDeviceDAL, DeviceDAL>();
builder.Services.AddScoped<ISnmpCredentialDAL, SnmpCredentialDAL>();
builder.Services.AddScoped<IParameterDAL, ParameterDAL>();
builder.Services.AddScoped<IDeviceParameterDAL, DeviceParameterDAL>();

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ISnmpCredentialService, SnmpCredentialService>();
builder.Services.AddScoped<ISnmpRequestedService, SnmpRequestedService>();
builder.Services.AddScoped<IPollingService, PollingService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IDeviceParameterService, DeviceParameterService>();

builder.Services.AddSingleton<IEventPublisher , RabbitMQEventPublisher>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapProfile>());

builder.Services.Configure<RabbitMQSetting>(
    builder.Configuration.GetSection(RabbitMQSetting.SectionName) 
);
builder.Services.Configure<ElasticSearchSettings>(builder.Configuration.GetSection("ElasticSearch"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AddDeviceDTOValidator>();

builder.Services.AddScoped<IOutboxMessageDAL, OutboxMessageDAL>();

builder.Services.AddScoped<IOutboxService, OutboxService>();
builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();

builder.Services.AddHostedService<OutboxBackgroundService>();

builder.Services.AddScoped<IDeviceQueryService, DeviceQueryService>();
builder.Services.AddScoped<ISnmpCredentialQueryService, SnmpCredentialQueryService>();
builder.Services.AddScoped<IDeviceParameterQueryService, DeviceParameterQueryService>();

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDAL, UserDAL>();

builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

var jwtSettings = builder.Configuration
    .GetSection("JWT")
    .Get<JWTSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings!.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "AIDCARE API",
        Version = "v1",
        Description = "Bu API, AIDCARE projesinin servislerini sunar."
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Lütfen 'Bearer ' ve ardından token'ı girin. Örnek: 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");

    return ConnectionMultiplexer.Connect(configuration);
});
var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
