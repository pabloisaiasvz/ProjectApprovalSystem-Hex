using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data.Repositories
{
    public class ProjectProposalRepository : IProjectProposalRepository
    {
        private readonly ProjectApprovalDbContext _context;

        public ProjectProposalRepository(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectProposal?> GetByIdAsync(Guid id)
        {
            return await _context.ProjectProposals
                .Include(p => p.ApprovalStatus)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(ProjectProposal proposal)
        {
            await _context.ProjectProposals.AddAsync(proposal);
        }


        public async Task UpdateAsync(ProjectProposal proposal)
        {
            _context.ProjectProposals.Update(proposal);
            await _context.SaveChangesAsync();
        }

        async Task<IEnumerable<ProjectProposal>> IProjectProposalRepository.GetAllAsync()
        {
            return await _context.ProjectProposals.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
