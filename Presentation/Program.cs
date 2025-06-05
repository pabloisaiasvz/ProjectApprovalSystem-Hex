using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ConsoleUI;
using Domain.Interfaces;

namespace Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("  SISTEMA DE APROBACIÓN DE PROYECTOS");
            Console.WriteLine("=============================================");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();

            services.AddDbContext<ProjectApprovalDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUserQueries, UserQueries>();
            services.AddScoped<IProjectProposalQueries, ProjectProposalQueries>();
            services.AddScoped<IPendingApprovalQueries, PendingApprovalQueries>();
            services.AddScoped<ICatalogQueries, CatalogQueries>();

            services.AddScoped<IProjectProposalRepository, ProjectProposalRepository>();
            services.AddScoped<IApprovalStatusRepository, ApprovalStatusRepository>();
            services.AddScoped<IApprovalRuleRepository, ApprovalRuleRepository>();
            services.AddScoped<IProjectApprovalStepRepository, ProjectApprovalStepRepository>();
            services.AddScoped<IApproverRoleRepository, ApproverRoleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProjectCreateService, ProjectCreateService>();
            services.AddScoped<IProjectApprovalProcessorService, ProjectApprovalProcessorService>();

            services.AddScoped<ProjectApprovalManager>();

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var manager = scope.ServiceProvider.GetRequiredService<ProjectApprovalManager>();
                    await manager.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            Console.WriteLine("Gracias por usar el sistema de aprobación de proyectos!");
            Console.ReadKey();
        }
    }
}