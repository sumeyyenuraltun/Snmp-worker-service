using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Infrastructure.Configuration
{
    public class ElasticSearchSettings
    {
        public string Uri { get; set; } = "http://localhost:9200";
        public string IndexFormat { get; set; } = "snmp-logs-{0:yyyy.MM.dd}";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
