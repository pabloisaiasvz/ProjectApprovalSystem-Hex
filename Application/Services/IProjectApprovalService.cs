using System;
using System.Collections.Generic;
using Application.DTOs;

namespace Application.Services
{
    public interface IProjectCreateService
    {
        Task<Guid?> CreateProjectProposalAsync(string title, string description, decimal estimatedAmount, int duration, int areaId, int projectTypeId, int userId);
    }
}
