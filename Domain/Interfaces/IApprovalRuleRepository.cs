using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IApprovalRuleRepository
    {
        Task<IEnumerable<ApprovalRule>> GetRulesAsync(decimal estimatedAmount, int areaId, int projectTypeId);
    }
}
