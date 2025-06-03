using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IApprovalStatusRepository
    {
        Task<ApprovalStatus?> GetByNameAsync(string name);
        Task<ApprovalStatus?> GetByIdAsync(int id);
    }
}
