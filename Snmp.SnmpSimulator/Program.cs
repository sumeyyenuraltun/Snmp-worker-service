using System.Net;
using System.Net.Sockets;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

const int Port = 1161;

// ---------------- v1 / v2c ayarlari ----------------
var community = new OctetString("public");

// ---------------- v3 ayarlari ----------------
// Engine ID sabit olmali (yeniden baslatmalar arasinda degisirse manager'lar sasirir).
var engineId = new OctetString(ByteTool.Convert("80004fb805636c6f75644dab22cc"));
var startTime = DateTime.UtcNow;

// v3 kullanicilari (kullanici adi / auth / priv sifrelerini kendine gore degistir)
var users = new UserRegistry();
users.Add(new OctetString("usr-none"), DefaultPrivacyProvider.DefaultPair);                    // noAuthNoPriv
users.Add(new OctetString("usr-sha"),
    new DefaultPrivacyProvider(new SHA1AuthenticationProvider(new OctetString("12345678")))); // authNoPriv
if (AESPrivacyProviderBase.IsSupported)
{
    users.Add(new OctetString("usr-sha-aes"),
        new AESPrivacyProvider(new OctetString("12345678"),
            new SHA1AuthenticationProvider(new OctetString("12345678"))));                    // authPriv
}

// usmStats sayaclari
uint statUnknownEngine = 0, statUnknownUser = 0, statNotInTime = 0,
     statAuthFail = 0, statBadLevel = 0, statDecrypt = 0;

var oidValues = new SortedDictionary<string, ISnmpData>
{
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.1.0"] = new OctetString("1"),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.2.0"] = new OctetString("2"),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.3.0"] = new OctetString("PROFEN"),
    ["1.3.6.1.4.1.3442.101.1.1045.2.10.4.0"] = new OctetString("4")
};

using var udp = new UdpClient(Port);
Console.WriteLine($"SNMP simulatoru UDP {Port} portunda dinliyor (v1/v2c/v3). Cikmak icin Ctrl+C.");

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
        // DIKKAT: v3 paketlerinin cozulebilmesi icin registry olarak "users" veriliyor.
        var messages = MessageFactory.ParseMessages(buffer, 0, buffer.Length, users);

        foreach (var message in messages)
        {
            if (message.Version == VersionCode.V3)
            {
                HandleV3(message, remote);
            }
            else
            {
                HandleCommunity(message, remote);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Paket islenemedi ({remote}): {ex.Message}");
    }
}

// ============================================================
// Ortak PDU isleme: GET / GETNEXT / SET hepsi burada
// ============================================================
(List<Variable> Vars, ErrorCode Error, int Index)? ProcessPdu(ISnmpPdu pdu, IPEndPoint remote)
{
    var result = new List<Variable>();

    switch (pdu.TypeCode)
    {
        case SnmpType.GetRequestPdu:
            foreach (var variable in pdu.Variables)
            {
                if (oidValues.TryGetValue(variable.Id.ToString(), out var value))
                {
                    result.Add(new Variable(variable.Id, value));
                    Console.WriteLine($"GET {variable.Id} => {value}");
                }
                else
                {
                    result.Add(new Variable(variable.Id, new NoSuchObject()));
                }
            }
            return (result, ErrorCode.NoError, 0);

        case SnmpType.GetNextRequestPdu:
            foreach (var variable in pdu.Variables)
            {
                var next = oidValues.Keys
                    .FirstOrDefault(x => string.CompareOrdinal(x, variable.Id.ToString()) > 0);

                if (next != null)
                {
                    result.Add(new Variable(new ObjectIdentifier(next), oidValues[next]));
                    Console.WriteLine($"GETNEXT {variable.Id} => {next}");
                }
                else
                {
                    result.Add(new Variable(variable.Id, new EndOfMibView()));
                }
            }
            return (result, ErrorCode.NoError, 0);

        case SnmpType.SetRequestPdu:
            foreach (var variable in pdu.Variables)
            {
                oidValues[variable.Id.ToString()] = variable.Data;
                result.Add(variable);
                Console.WriteLine($"SET {variable.Id} = {variable.Data}");
            }
            return (result, ErrorCode.NoError, 0);

        default:
            Console.WriteLine($"{remote} -> Desteklenmeyen PDU: {pdu.TypeCode}");
            return null;
    }
}

// ============================================================
// v1 / v2c (community tabanli)
// ============================================================
void HandleCommunity(ISnmpMessage message, IPEndPoint remote)
{
    // Community kontrolu (istemiyorsan bu blogu kaldir)
    if (message.Parameters.UserName != community)
    {
        Console.WriteLine($"{remote} -> Gecersiz community: {message.Parameters.UserName}");
        return;
    }

    var processed = ProcessPdu(message.Pdu(), remote);
    if (processed == null)
    {
        return;
    }

    var (vars, error, index) = processed.Value;

    var response = new ResponseMessage(
        message.RequestId(),
        message.Version,
        message.Parameters.UserName,
        error,
        index,
        vars);

    var bytes = response.ToBytes();
    udp.Send(bytes, bytes.Length, remote);
}

// ============================================================
// v3 (USM: discovery, auth, priv)
// ============================================================
void HandleV3(ISnmpMessage message, IPEndPoint remote)
{
    var parameters = message.Parameters;

    // Kullanici kayitli mi? (Discovery mesajlarinda kullanici adi bos gelir,
    // Find bos ad icin DefaultPair dondurur, o yuzden discovery buradan gecer.)
    var privacy = users.Find(parameters.UserName);
    if (privacy == null)
    {
        Console.WriteLine($"{remote} -> v3 bilinmeyen kullanici: {parameters.UserName}");
        SendReport(message, remote,
            new Variable(Messenger.UnknownSecurityName, new Counter32(statUnknownUser++)));
        return;
    }

    // Sifre yanlissa PDU cozulemez ve tip Unknown olur.
    if (message.TypeCode() == SnmpType.Unknown)
    {
        Console.WriteLine($"{remote} -> v3 decrypt hatasi (priv sifresi yanlis olabilir)");
        SendReport(message, remote,
            new Variable(Messenger.DecryptionError, new Counter32(statDecrypt++)));
        return;
    }

    // Discovery: engine id bos gelir -> engine id + boots + time iceren Report doneriz.
    if (parameters.EngineId.GetRaw().Length == 0)
    {
        Console.WriteLine($"{remote} -> v3 discovery");
        SendDiscoveryReport(message, remote);
        return;
    }

    if (parameters.EngineId != engineId)
    {
        SendReport(message, remote,
            new Variable(Messenger.UnknownEngineId, new Counter32(statUnknownEngine++)));
        return;
    }

    // Auth imzasi dogru mu?
    if (parameters.IsInvalid)
    {
        Console.WriteLine($"{remote} -> v3 auth hatasi (auth sifresi yanlis)");
        SendReport(message, remote,
            new Variable(Messenger.AuthenticationFailure, new Counter32(statAuthFail++)));
        return;
    }

    // Istekteki guvenlik seviyesi kullanicinin seviyesiyle uyusuyor mu?
    if ((privacy.ToSecurityLevel() | Levels.Reportable) != message.Header.SecurityLevel)
    {
        SendReport(message, remote,
            new Variable(Messenger.UnsupportedSecurityLevel, new Counter32(statBadLevel++)));
        return;
    }

    // Zaman penceresi kontrolu (RFC 3414, +/- 150 sn)
    var time = EngineTimeData();
    if (!IsInTime(time, parameters.EngineBoots.ToInt32(), parameters.EngineTime.ToInt32()))
    {
        SendReport(message, remote,
            new Variable(Messenger.NotInTimeWindow, new Counter32(statNotInTime++)));
        return;
    }

    var processed = ProcessPdu(message.Pdu(), remote);
    if (processed == null)
    {
        return;
    }

    var (vars, error, index) = processed.Value;

    var response = new ResponseMessage(
        VersionCode.V3,
        new Header(
            new Integer32(message.MessageId()),
            new Integer32(Messenger.MaxMessageSize),
            privacy.ToSecurityLevel()),
        new SecurityParameters(
            parameters.EngineId,
            new Integer32(time[0]),
            new Integer32(time[1]),
            parameters.UserName,
            privacy.AuthenticationProvider.CleanDigest,
            privacy.Salt),
        new Scope(
            message.Scope.ContextEngineId,
            message.Scope.ContextName,
            new ResponsePdu(message.RequestId(), error, index, vars)),
        privacy,
        true,
        null);

    var bytes = response.ToBytes();
    udp.Send(bytes, bytes.Length, remote);
}

void SendDiscoveryReport(ISnmpMessage request, IPEndPoint remote)
{
    var pair = DefaultPrivacyProvider.DefaultPair;
    var time = EngineTimeData();

    var report = new ReportMessage(
        VersionCode.V3,
        new Header(
            new Integer32(request.MessageId()),
            new Integer32(Messenger.MaxMessageSize),
            0),
        new SecurityParameters(
            engineId,
            new Integer32(time[0]),
            new Integer32(time[1]),
            OctetString.Empty,
            OctetString.Empty,
            OctetString.Empty),
        new Scope(
            engineId,
            request.Scope?.ContextName ?? OctetString.Empty,
            new ReportPdu(
                request.RequestId(),
                ErrorCode.NoError,
                0,
                new List<Variable> { new(Messenger.UnknownEngineId, new Counter32(statUnknownEngine++)) })),
        pair,
        null);

    var bytes = report.ToBytes();
    udp.Send(bytes, bytes.Length, remote);
}

void SendReport(ISnmpMessage request, IPEndPoint remote, Variable failure)
{
    var pair = DefaultPrivacyProvider.DefaultPair;
    var time = EngineTimeData();

    var report = new ReportMessage(
        request.Version,
        new Header(
            new Integer32(request.MessageId()),
            new Integer32(Messenger.MaxMessageSize),
            0),
        new SecurityParameters(
            engineId,
            new Integer32(time[0]),
            new Integer32(time[1]),
            request.Parameters.UserName,
            pair.AuthenticationProvider.CleanDigest,
            pair.Salt),
        new Scope(
            request.Scope?.ContextEngineId ?? OctetString.Empty,
            request.Scope?.ContextName ?? OctetString.Empty,
            new ReportPdu(
                request.RequestId(),
                ErrorCode.NoError,
                0,
                new List<Variable> { failure })),
        pair,
        null);

    var bytes = report.ToBytes();
    udp.Send(bytes, bytes.Length, remote);
}

// engineBoots / engineTime hesabi
int[] EngineTimeData()
{
    var seconds = (DateTime.UtcNow - startTime).Ticks / 10_000_000;
    return new[] { (int)(seconds / int.MaxValue), (int)(seconds % int.MaxValue) };
}

static bool IsInTime(int[] current, int pastBoots, int pastTime)
{
    if (current[0] == int.MaxValue) return false;
    if (current[0] != pastBoots) return false;
    if (current[1] == pastTime) return true;
    return Math.Abs(current[1] - pastTime) <= 150;
}