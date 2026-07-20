using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNMP.BLL.Abstract;
using SNMP.ENTITY.Concrete;

namespace SNMP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnmpLogController : BaseController<SnmpLog>
    {
        public SnmpLogController(IBaseService<SnmpLog> baseService) : base(baseService)
        {
        }
    }
}
