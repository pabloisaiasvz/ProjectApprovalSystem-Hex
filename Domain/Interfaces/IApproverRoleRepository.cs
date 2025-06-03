using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IApproverRoleRepository
    {
        Task<ApproverRole?> GetByIdAsync(int id);
    }
}
