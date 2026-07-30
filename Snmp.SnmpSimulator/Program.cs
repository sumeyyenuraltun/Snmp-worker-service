using System.Net;
using System.Net.Sockets;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

const int Port = 1161;
var rnd = new Random();

string Random(params int[] kodlar)
{
    var secilen = kodlar[rnd.Next(kodlar.Length)];
    return $"{secilen}";
}

var oidValues = new Dictionary<string, Func<ISnmpData>>
{
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.1.0"] = () => new OctetString("1"),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.2.0"] = () => new OctetString(Random(1, 2)),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.3.0"] = () => new OctetString("PROFEN"),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.4.0"] = () => new OctetString(Random(3, 4)),
};

using var udp = new UdpClient(Port);
Console.WriteLine($"SNMP simulatoru UDP {Port} portunda dinliyor. Cikmak icin Ctrl+C.");

while (true)
{
    var remote = new IPEndPoint(IPAddress.Any, 0);
    byte[] buffer;

    try
    {
        buffer = udp.Receive(ref remote);
    }
    catch (SocketException ex)
    {
        Console.WriteLine($"Soket hatasi: {ex.Message}");
        continue;
    }

    try
    {
        var messages = MessageFactory.ParseMessages(
            buffer, 0, buffer.Length, new UserRegistry());

        foreach (var message in messages)
        {
            var pdu = message.Pdu();

            if (pdu.TypeCode != SnmpType.GetRequestPdu)
            {
                Console.WriteLine($"{remote} -> desteklenmeyen PDU tipi: {pdu.TypeCode}");
                continue;
            }

            var responseVariables = new List<Variable>();

            foreach (var variable in pdu.Variables)
            {
                var oid = variable.Id.ToString();

                if (oidValues.TryGetValue(oid, out var uret))
                {
                    var data = uret();
                    responseVariables.Add(new Variable(variable.Id, data));
                    Console.WriteLine($"{remote} -> GET {oid} => {data}");
                }
                else
                {
                    responseVariables.Add(new Variable(variable.Id, new NoSuchObject()));
                    Console.WriteLine($"{remote} -> GET {oid} => (boyle bir OID yok)");
                }
            }

            var response = new ResponseMessage(
                message.RequestId(),
                message.Version,
                message.Parameters.UserName, 
                ErrorCode.NoError,
                0,
                responseVariables);

            var bytes = response.ToBytes();
            udp.Send(bytes, bytes.Length, remote);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Paket islenemedi ({remote}): {ex.Message}");
    }
}