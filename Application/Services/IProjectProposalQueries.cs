using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProjectProposalQueries
    {
        Task<List<ProjectProposalSummaryDto>> GetProjectsByUserIdAsync(int userId);
        Task<ProjectProposalDto> GetProjectDetailsByIdAsync(Guid projectId);
        Task<List<ProjectApprovalStepDto>> GetProjectApprovalStepsAsync(Guid projectId);
    }
}
