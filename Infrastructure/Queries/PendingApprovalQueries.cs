using Application.DTOs;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class PendingApprovalQueries : IPendingApprovalQueries
    {
        private readonly ProjectApprovalDbContext _context;

        public PendingApprovalQueries(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingApprovalDto>> GetPendingApprovalsByRoleAsync(int roleId)
        {
            var pendingStatusId = await _context.ApprovalStatuses
                .Where(st => st.Name == "Pending")
                .Select(st => st.Id)
                .FirstOrDefaultAsync();

            if (pendingStatusId == 0)
                return new List<PendingApprovalDto>();

            return await _context.ProjectApprovalSteps
                .Include(s => s.ProjectProposal)
                .Include(s => s.ApproverRole)
                .Where(s => s.ApproverRoleId == roleId && s.Status == pendingStatusId)
                .OrderBy(s => s.ProjectProposal.CreatedAt)
                .Select(s => new PendingApprovalDto
                {
                    StepId = s.Id,
                    ProjectProposalId = s.ProjectProposalId,
                    ProjectTitle = s.ProjectProposal.Title,
                    EstimatedAmount = s.ProjectProposal.EstimatedAmount,
                    StepOrder = s.StepOrder,
                    ApproverRoleName = s.ApproverRole.Name,
                    CreatedAt = s.ProjectProposal.CreatedAt
                })
                .ToListAsync();
        }
    }
}