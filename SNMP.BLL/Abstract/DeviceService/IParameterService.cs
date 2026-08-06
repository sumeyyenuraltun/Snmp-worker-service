using Snmp.Business.DTOs.Parameter;
using Snmp.Business.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract.DeviceService
{
    public interface IParameterService
    {
        Task<Result<List<ParameterDTO>>> GetAllAsync();

        Task<Result<ParameterDTO>> GetByIdAsync(int id);

        Task<Result> AddAsync(AddParameterDTO addParameterDTO, CancellationToken cancellationToken);

        Task<Result> UpdateAsync(UpdateParameterDTO updateParameterDTO, CancellationToken cancellationToken);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
