using Application.DTOs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Services;

namespace Infrastructure.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly ProjectApprovalDbContext _context;

        public UserQueries(ProjectApprovalDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    RoleName = _context.ApproverRoles
                        .Where(r => r.Id == u.Role)
                        .Select(r => r.Name)
                        .FirstOrDefault() ?? "Sin rol"
                })
                .ToListAsync();
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    RoleName = _context.ApproverRoles
                        .Where(r => r.Id == u.Role)
                        .Select(r => r.Name)
                        .FirstOrDefault() ?? "Sin rol"
                })
                .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception($"Usuario con Id {userId} no encontrado.");

            return user;
        }
    }
}