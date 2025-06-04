using Application.DTOs;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class CatalogQueries : ICatalogQueries
    {
        private readonly ProjectApprovalDbContext _context;

        public CatalogQueries(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<List<AreaDto>> GetAllAreasAsync()
        {
            return await _context.Areas
                .Select(a => new AreaDto
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }

        public async Task<List<ProjectTypeDto>> GetAllProjectTypesAsync()
        {
            return await _context.ProjectTypes
                .Select(pt => new ProjectTypeDto
                {
                    Id = pt.Id,
                    Name = pt.Name
                })
                .ToListAsync();
        }

        public async Task<List<ApprovalStatusDto>> GetAllApprovalStatusesAsync()
        {
            return await _context.ApprovalStatuses
                .Select(s => new ApprovalStatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        public async Task<ApprovalStatusDto> GetApprovalStatusByNameAsync(string statusName)
        {
            var status = await _context.ApprovalStatuses
                .Where(s => s.Name == statusName)
                .Select(s => new ApprovalStatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .FirstOrDefaultAsync();

            return status ?? throw new Exception($"Estado de aprobación '{statusName}' no encontrado.");
        }

    }
}