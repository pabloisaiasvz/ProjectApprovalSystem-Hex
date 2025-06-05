using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Data
{
    public class ProjectApprovalDbContextFactory : IDesignTimeDbContextFactory<ProjectApprovalDbContext>
    {
        public ProjectApprovalDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation"))
                .AddJsonFile("AppSettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ProjectApprovalDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ProjectApprovalDbContext(builder.Options);
        }
    }
}
