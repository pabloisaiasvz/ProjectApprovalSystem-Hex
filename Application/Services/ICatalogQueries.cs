using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICatalogQueries
    {
        Task<List<AreaDto>> GetAllAreasAsync();
        Task<List<ProjectTypeDto>> GetAllProjectTypesAsync();
        Task<List<ApprovalStatusDto>> GetAllApprovalStatusesAsync();
        Task<ApprovalStatusDto> GetApprovalStatusByNameAsync(string statusName);
    }
}
