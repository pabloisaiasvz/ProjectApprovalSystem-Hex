using Application.DTOs;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


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
            var pendingStatusIds = await _context.ApprovalStatuses
                .Where(st => st.Name == "Pending" || st.Name == "Observed")
                .Select(st => st.Id)
                .ToListAsync();

            if (pendingStatusIds == null || pendingStatusIds.Count == 0)
                return new List<PendingApprovalDto>();

            return await _context.ProjectApprovalSteps
                .Include(s => s.ProjectProposal)
                .Include(s => s.ApproverRole)
                .Where(s => s.ApproverRoleId == roleId && pendingStatusIds.Contains(s.Status))
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