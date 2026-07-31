using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Parameter;
using Snmp.DataAccess.Abstract;
using Snmp.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Concrete
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterDAL _parameterDAL;
        private readonly IMapper _mapper;

        public ParameterService(IParameterDAL parameterDAL, IMapper mapper)
        {
            _parameterDAL = parameterDAL;
            _mapper = mapper;
        }

        public async Task<List<ParameterDTO>> GetAllAsync()
        {
            var parameters = await _parameterDAL.GetAllAsync();

            return _mapper.Map<List<ParameterDTO>>(parameters);
        }

        public async Task<ParameterDTO?> GetByIdAsync(int id)
        {
            var parameter = await _parameterDAL.GetByIdAsync(id);

            return _mapper.Map<ParameterDTO>(parameter);
        }

        public async Task<ParameterDTO> AddAsync(AddParameterDTO addParameterDTO, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Parameter>(addParameterDTO);

            await _parameterDAL.AddAsync(entity, cancellationToken);

            return _mapper.Map<ParameterDTO>(entity);
        }

        public async Task<ParameterDTO> UpdateAsync(UpdateParameterDTO updateParameterDTO)
        {
            var entity = await _parameterDAL.GetByIdAsync(updateParameterDTO.Id);

            if (entity == null)
                throw new Exception("Parameter not found.");

            _mapper.Map(updateParameterDTO, entity);

            await _parameterDAL.UpdateAsync(entity);

            return _mapper.Map<ParameterDTO>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _parameterDAL.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Parameter not found.");

            await _parameterDAL.DeleteAsync(entity);
        }
    }
}
