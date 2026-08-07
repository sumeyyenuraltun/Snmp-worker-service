using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Snmp.Business.Abstract.Auth;
using Snmp.Business.Abstract.DeviceService;
using Snmp.Business.Abstract.Outbox;
using Snmp.Business.Abstract.Security;
using Snmp.Business.Abstract.Snmp;
using Snmp.Business.Abstract.UserService;
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
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.Entity.Abstract;
using Snmp.Infrastructure.Configuration;
using Snmp.Infrastructure.Messaging;
using Snmp.Infrastructure.Outbox;
using Snmp.WebAPI.Configuration;
using Snmp.WebAPI.Middlewares;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) =>
{
    var settings = context.Configuration.GetSection("ElasticSearch").Get<ElasticSearchSettings>();

    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console()
                 .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(settings.Uri))
                 {
                     AutoRegisterTemplate = true,
                     IndexFormat = settings.IndexFormat
                 });
});


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>option.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));


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

builder.Services.AddAuthorization();
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
var app = builder.Build();

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
