using Snmp.Business.DTOs.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snmp.Business.Abstract
{
    public interface IParameterService
    {
        Task<List<ParameterDTO>> GetAllAsync();

        Task<ParameterDTO?> GetByIdAsync(int id);

        Task<ParameterDTO> AddAsync(AddParameterDTO addParameterDTO, CancellationToken cancellationToken);

        Task<ParameterDTO> UpdateAsync(UpdateParameterDTO updateParameterDTO);

        Task DeleteAsync(int id);
    }
}
