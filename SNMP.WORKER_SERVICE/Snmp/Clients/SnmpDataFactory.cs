using Lextm.SharpSnmpLib;
using Snmp.EventWorker.Snmp.Models;
using SNMP.ENTITY.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.EventWorker.Snmp.Clients
{
    public static class SnmpDataFactory
    {
        public static ISnmpData Create(SnmpRequest request)
        {
            return request.DataType switch
            {
                SnmpDataType.Integer =>
                    new Integer32(int.Parse(request.Value ?? "0")),

                SnmpDataType.OctetString =>
                    new OctetString(request.Value ?? string.Empty),

                SnmpDataType.Gauge32 =>
                    new Gauge32(uint.Parse(request.Value ?? "0")),

                SnmpDataType.Counter32 =>
                    new Counter32(uint.Parse(request.Value ?? "0")),

                SnmpDataType.TimeTicks =>
                    new TimeTicks(uint.Parse(request.Value ?? "0")),

                _ => throw new NotSupportedException()
            };
        }
    }
}
