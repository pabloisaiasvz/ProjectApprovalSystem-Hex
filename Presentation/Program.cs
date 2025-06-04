using Infrastructure.Data;
using Presentation.ConsoleUI;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Queries;

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

            var optionsBuilder = new DbContextOptionsBuilder<ProjectApprovalDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            try
            {
                using (var context = new ProjectApprovalDbContext(optionsBuilder.Options))
                {
                    context.Database.EnsureCreated();

                    var userQueries = new UserQueries(context);
                    var projectProposalQueries = new ProjectProposalQueries(context);
                    var pendingApprovalQueries = new PendingApprovalQueries(context);
                    var catalogQueries = new CatalogQueries(context);

                    var proposalRepo = new ProjectProposalRepository(context);
                    var statusRepo = new ApprovalStatusRepository(context);
                    var ruleRepo = new ApprovalRuleRepository(context);
                    var stepRepo = new ProjectApprovalStepRepository(context);
                    var roleRepo = new ApproverRoleRepository(context);
                    var unitOfWork = new UnitOfWork(context);

                    var approvalService = new ProjectApprovalService(
                        proposalRepo, statusRepo, ruleRepo, stepRepo, roleRepo, unitOfWork);

                    var manager = new ProjectApprovalManager(
                        userQueries,
                        projectProposalQueries,
                        pendingApprovalQueries,
                        catalogQueries,
                        approvalService);

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