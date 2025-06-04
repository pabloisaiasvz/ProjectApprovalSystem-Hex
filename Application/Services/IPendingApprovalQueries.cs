using Application.DTOs;

namespace Application.Services
{
    public interface IPendingApprovalQueries
    {
        Task<List<PendingApprovalDto>> GetPendingApprovalsByRoleAsync(int roleId);
    }
}
