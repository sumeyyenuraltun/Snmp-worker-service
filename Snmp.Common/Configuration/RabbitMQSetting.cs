using System;
using System.Collections.Generic;
using System.Text;


namespace Snmp.Common.Configuration
{
    public class RabbitMQSetting
    {
        public const string SectionName = "RabbitMQ";
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public required string UserName { get; set; }
        public required string Password { get; set;}
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "snmp-project-events";
        public string QueueName { get; set; } = "snmp-project-queue";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
    }
}

