using Snmp.Entity.Concrete;

namespace SNMP.ENTITY.Concrete
{
    public class Device : BaseEntity
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public int Port { get; set; }
        public ICollection<SnmpCredential> Credentials { get; set; }= new List<SnmpCredential>();
        public ICollection<DeviceParameter> DeviceParametres { get; set; } = new List<DeviceParameter>();
        
    }
}
