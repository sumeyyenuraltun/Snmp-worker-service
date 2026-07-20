using Microsoft.EntityFrameworkCore;
using SNMP.BLL.Abstract;
using SNMP.BLL.Concrete;
using SNMP.DAL.Abstract;
using SNMP.DAL.Concrete;
using SNMP.DAL.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>option.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

builder.Services.AddScoped<IDeviceDAL, DeviceDAL>();
builder.Services.AddScoped<ISnmpLogDAL, SnmpLogDAL>();

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ISnmpLogService, SnmpLogService>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); ;

var app = builder.Build();

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
