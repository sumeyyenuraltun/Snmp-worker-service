using Microsoft.AspNetCore.Mvc;
using SNMP.BLL.Abstract;
using SNMP.ENTITY.Concrete;

namespace SNMP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : BaseEntity
    {
        private readonly IBaseService<TEntity> _baseService;

        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var entities = _baseService.GetAll();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _baseService.GetById(id);
            if (entity == null)
            {
                return NotFound(); 
            }
            return Ok(entity); 
        }

        [HttpPost]
        public IActionResult Post([FromBody] TEntity entity)
        {
            _baseService.Add(entity);
            return Ok(entity);
        }

        [HttpPut]
        public IActionResult Put([FromBody] TEntity entity)
        {
            _baseService.Update(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
           
            var entity = _baseService.GetById(id);
            if (entity == null)
            {
                return NotFound();
            }

            _baseService.Delete(entity);
            return Ok("Silme işlemi başarılı.");
        }
    }
}
