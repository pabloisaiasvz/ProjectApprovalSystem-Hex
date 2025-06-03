using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ApprovalStatusRepository : IApprovalStatusRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ApprovalStatusRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalStatus?> GetByIdAsync(int id)
        {
            return await _context.ApprovalStatuses.FindAsync(id);
        }

        public async Task<ApprovalStatus?> GetByNameAsync(string name)
        {
            return await _context.ApprovalStatuses.FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}
