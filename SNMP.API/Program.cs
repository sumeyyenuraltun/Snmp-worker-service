using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Snmp.Business.Mapping;
using Snmp.Infrastructure.Configuration;
using Snmp.Infrastructure.Messaging;
using SNMP.BLL.Abstract;
using SNMP.BLL.Concrete;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;
using SNMP.ENTITY.Abstract;
using StackExchange.Redis;
using Serilog;
using Snmp.WebAPI.Middlewares;
using Snmp.DataAccess.Abstract;
using Snmp.DataAccess.Concrete;
using Snmp.Business.Abstract;
using Snmp.Business.Concrete;
using Snmp.Business.ValidationRules.DeviceValidator;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AddDeviceDTOValidator>();

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

builder.Host.UseSerilog();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); 

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

app.UseAuthorization();

app.MapControllers();

app.Run();
