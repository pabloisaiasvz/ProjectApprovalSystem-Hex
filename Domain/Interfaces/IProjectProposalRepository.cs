using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProjectProposalRepository
    {
        Task<ProjectProposal?> GetByIdAsync(Guid id);
        Task AddAsync(ProjectProposal proposal);
        Task UpdateAsync(ProjectProposal proposal);
        Task<IEnumerable<ProjectProposal>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
