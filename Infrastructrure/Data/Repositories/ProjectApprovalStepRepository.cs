using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ProjectApprovalStepRepository : IProjectApprovalStepRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectApprovalStepRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectApprovalStep?> GetByProposalIdAndStepOrderAsync(Guid proposalId, int stepOrder)
        {
            return await _context.ProjectApprovalSteps
                .Include(s => s.ApprovalStatus)
                .FirstOrDefaultAsync(s => s.ProjectProposalId == proposalId && s.StepOrder == stepOrder);
        }

        public async Task<IEnumerable<ProjectApprovalStep>> GetByProposalIdAsync(Guid proposalId)
        {
            return await _context.ProjectApprovalSteps
                .Where(s => s.ProjectProposalId == proposalId)
                .Include(s => s.ApprovalStatus)
                .ToListAsync();
        }

        public async Task AddAsync(ProjectApprovalStep step)
        {
            await _context.ProjectApprovalSteps.AddAsync(step);
        }

        public async Task UpdateAsync(ProjectApprovalStep step)
        {
            _context.ProjectApprovalSteps.Update(step);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
