using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ApproverRoleRepository : IApproverRoleRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ApproverRoleRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<ApproverRole?> GetByIdAsync(int id)
        {
            return await _context.ApproverRoles.FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
