using System;
using System.Collections.Generic;
using Application.DTOs;

namespace Application.Services
{
    public interface IProjectApprovalService
    {
        Task<Guid?> CreateProjectProposalAsync(string title, string description, decimal estimatedAmount, int duration, int areaId, int projectTypeId, int userId);
        Task ProcessApprovalAsync(Guid projectId, bool isApproved, int stepOrder, int userId, string? observations = null);
        Task UpdateProjectStatusAsync(Guid projectId, int statusId);
    }
}
