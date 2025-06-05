using Application.DTOs;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class ProjectProposalQueries : IProjectProposalQueries
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectProposalQueries(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectProposalSummaryDto>> GetProjectsByUserIdAsync(int userId)
        {
            var projects = await _context.ProjectProposals
                .Where(p => p.CreatedBy == userId)
                .Include(p => p.Areas)
                .Include(p => p.ProjectType)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            var result = new List<ProjectProposalSummaryDto>();

            var rejectedStatusId = await _context.ApprovalStatuses
                .Where(a => a.Name == "Rejected")
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            var approvedStatusId = await _context.ApprovalStatuses
                .Where(a => a.Name == "Approved")
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            var observedStatusId = await _context.ApprovalStatuses
                .Where(a => a.Name == "Observed")
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            foreach (var project in projects)
            {
                var steps = await _context.ProjectApprovalSteps
                    .Where(s => s.ProjectProposalId == project.Id)
                    .ToListAsync();

                string status;

                if (steps.Any(s => s.Status == rejectedStatusId))
                    status = "Rejected";
                else if (steps.Any(s => s.Status == observedStatusId))
                    status = "Observed";
                else if (steps.All(s => s.Status == approvedStatusId))
                    status = "Approved";
                else
                    status = "Pending";

                var areaName = await _context.Areas
                    .Where(a => a.Id == project.Area)
                    .Select(a => a.Name)
                    .FirstOrDefaultAsync() ?? "N/A";

                var typeName = await _context.ProjectTypes
                    .Where(t => t.Id == project.Type)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync() ?? "N/A";

                result.Add(new ProjectProposalSummaryDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    EstimatedAmount = project.EstimatedAmount,
                    AreaName = areaName,
                    TypeName = typeName,
                    Status = status,
                    CreatedAt = project.CreatedAt
                });
            }

            return result;
        }

        public async Task<ProjectProposalDto> GetProjectDetailsByIdAsync(Guid projectId)
        {
            var project = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return null;

            var areaName = await _context.Areas
                .Where(a => a.Id == project.Area)
                .Select(a => a.Name)
                .FirstOrDefaultAsync() ?? "N/A";

            var typeName = await _context.ProjectTypes
                .Where(t => t.Id == project.Type)
                .Select(t => t.Name)
                .FirstOrDefaultAsync() ?? "N/A";

            var statusName = await _context.ApprovalStatuses
                .Where(s => s.Id == project.Status)
                .Select(s => s.Name)
                .FirstOrDefaultAsync() ?? "N/A";

            return new ProjectProposalDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                EstimatedAmount = project.EstimatedAmount,
                EstimatedDuration = project.EstimatedDuration,
                Area = project.Area,
                AreaName = areaName,
                Type = project.Type,
                TypeName = typeName,
                Status = project.Status,
                StatusName = statusName,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task<List<ProjectApprovalStepDto>> GetProjectApprovalStepsAsync(Guid projectId)
        {
            return await _context.ProjectApprovalSteps
                .Include(s => s.ApproverRole)
                .Where(s => s.ProjectProposalId == projectId)
                .OrderBy(s => s.StepOrder)
                .Select(s => new ProjectApprovalStepDto
                {
                    Id = s.Id,
                    ProjectProposalId = s.ProjectProposalId,
                    StepOrder = s.StepOrder,
                    ApproverRoleId = s.ApproverRoleId,
                    ApproverRoleName = s.ApproverRole.Name ?? "N/A",
                    Status = s.Status,
                    StatusName = _context.ApprovalStatuses
                        .Where(st => st.Id == s.Status)
                        .Select(st => st.Name)
                        .FirstOrDefault() ?? "N/A",
                    DecisionDate = s.DecisionDate,
                    ApprovedBy = s.ApproverRoleId,
                    Observations = s.Observations
                })
                .ToListAsync();
        }
    }
}