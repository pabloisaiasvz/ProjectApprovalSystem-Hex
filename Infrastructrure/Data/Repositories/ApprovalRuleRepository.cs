using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ApprovalRuleRepository : IApprovalRuleRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ApprovalRuleRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalRule>> GetRulesAsync(decimal estimatedAmount, int areaId, int projectTypeId)
        {
            return await _context.ApprovalRules
                .Where(r =>
                    r.MinAmount <= estimatedAmount &&
                    (r.MaxAmount == 0 || r.MaxAmount >= estimatedAmount) &&
                    (r.Area == null || r.Area == areaId) &&
                    (r.Type == null || r.Type == projectTypeId))
                .ToListAsync();
        }
    }
}
