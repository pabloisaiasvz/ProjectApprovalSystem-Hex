using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProjectApprovalProcessorService
    {
        Task ProcessApprovalAsync(Guid projectId, bool isApproved, int stepOrder, int userId, string? observations = null);
        Task<List<ProjectProposal>> GetProposalsByStatusNameAsync(string statusName);
        Task MarkAsObservedAsync(Guid projectId);
    }

}
