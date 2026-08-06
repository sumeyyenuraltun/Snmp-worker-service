using Microsoft.EntityFrameworkCore;
using Snmp.Entity.Concrete;
using SNMP.ENTITY.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMP.DAL.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<SnmpCredential> SnmpCredentials { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<DeviceParameter> DeviceParameters { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
