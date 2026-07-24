using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Snmp.Business.DTOs.SnmpLogs;
using SNMP.BLL.Abstract;
using SNMP.ENTITY.Concrete;

namespace SNMP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnmpLogController : ControllerBase
    {
        private readonly ISnmpLogService _snmpLogService;

        public SnmpLogController(ISnmpLogService snmpLogService)
        {
            _snmpLogService = snmpLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddSnmpLogDTO addSnmpLogDTO, CancellationToken cancellationToken)
        {
            await _snmpLogService.AddAsync(addSnmpLogDTO, cancellationToken);
            return Ok(new { Message = "Log başarıyla kayıt edildi ve kuyruğa event iletildi"});
        }

        [HttpGet]
        public async Task<IActionResult> GetLastLogs([FromBody] int count =100)
        {
            var logs = await _snmpLogService.GetLastLogsAsync(count);
            return Ok(logs);
        }

    }
}
