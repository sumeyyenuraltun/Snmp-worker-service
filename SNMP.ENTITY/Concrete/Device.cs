using Snmp.Entity.Concrete;

namespace SNMP.ENTITY.Concrete
{
    public class Device : BaseEntity
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public int Port { get; set; }
        public SnmpCredential Credential { get; set; }
        public ICollection<DeviceParameter> DeviceParametres { get; set; } = new List<DeviceParameter>();
        
    }
}
