using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProjectApprovalStepRepository
    {
        Task<ProjectApprovalStep?> GetByProposalIdAndStepOrderAsync(Guid proposalId, int stepOrder);
        Task<IEnumerable<ProjectApprovalStep>> GetByProposalIdAsync(Guid proposalId);
        Task AddAsync(ProjectApprovalStep step);
        Task UpdateAsync(ProjectApprovalStep step);
        Task SaveChangesAsync();
    }
}
