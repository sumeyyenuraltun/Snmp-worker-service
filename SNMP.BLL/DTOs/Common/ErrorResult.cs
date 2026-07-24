using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.DTOs.Common
{
    public class ErrorResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
