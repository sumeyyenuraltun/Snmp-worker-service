using AutoMapper;
using Snmp.Business.Abstract;
using Snmp.Business.DTOs.Parameter;
using Snmp.Business.Results;
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
        private readonly IUnitOfWork _unitOfWork;
        public ParameterService(IParameterDAL parameterDAL, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _parameterDAL = parameterDAL;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<ParameterDTO>>> GetAllAsync()
        {
            var parameters = await _parameterDAL.GetAllAsync();

            var dto = _mapper.Map<List<ParameterDTO>>(parameters);

            return Result<List<ParameterDTO>>.Success(dto);
        }

        public async Task<Result<ParameterDTO>> GetByIdAsync(int id)
        {
            var entity = await _parameterDAL.GetByIdAsync(id);

            if (entity == null)
                return Result<ParameterDTO>.Failure("Parameter not found.");

            var dto = _mapper.Map<ParameterDTO>(entity);

            return Result<ParameterDTO>.Success(dto);
        }

        public async Task<Result> AddAsync(AddParameterDTO addParameterDTO, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Parameter>(addParameterDTO);

            await _parameterDAL.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(UpdateParameterDTO updateParameterDTO, CancellationToken cancellationToken)
        {
            var entity = await _parameterDAL.GetByIdAsync(updateParameterDTO.Id);

            if (entity == null)
                return Result.Failure("Parameter not found.");

            _mapper.Map(updateParameterDTO, entity);

            await _parameterDAL.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _parameterDAL.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure("Parameter not found.");

            await _parameterDAL.DeleteAsync(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
