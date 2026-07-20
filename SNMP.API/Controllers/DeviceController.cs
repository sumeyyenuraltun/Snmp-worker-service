using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNMP.BLL.Abstract;
using SNMP.ENTITY.Concrete;

namespace SNMP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : BaseController<Device>
    {
        public DeviceController(IBaseService<Device> baseService) : base(baseService)
        {
        }
    }
}
