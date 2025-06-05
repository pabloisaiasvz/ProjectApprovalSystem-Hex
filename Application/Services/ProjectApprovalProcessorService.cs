using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ProjectApprovalProcessorService : IProjectApprovalProcessorService
    {
        private readonly IProjectProposalRepository _proposalRepo;
        private readonly IApprovalStatusRepository _statusRepo;
        private readonly IProjectApprovalStepRepository _stepRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectApprovalProcessorService(
            IProjectProposalRepository proposalRepo,
            IApprovalStatusRepository statusRepo,
            IProjectApprovalStepRepository stepRepo,
            IUnitOfWork unitOfWork)
        {
            _proposalRepo = proposalRepo;
            _statusRepo = statusRepo;
            _stepRepo = stepRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessApprovalAsync(Guid projectId, bool isApproved, int stepOrder, int userId, string? observations = null)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var projectProposal = await _proposalRepo.GetByIdAsync(projectId)
                    ?? throw new Exception("Proyecto no encontrado.");

                var currentStep = await _stepRepo.GetByProposalIdAndStepOrderAsync(projectId, stepOrder)
                    ?? throw new Exception("Paso de aprobación no encontrado.");

                var statusApproved = await _statusRepo.GetByNameAsync("Approved");
                var statusRejected = await _statusRepo.GetByNameAsync("Rejected");
                if (statusApproved == null || statusRejected == null)
                    throw new Exception("Estados 'Approved' o 'Rejected' no encontrados.");

                var allSteps = (await _stepRepo.GetByProposalIdAsync(projectId)).ToList();

                bool previousStepsApproved = allSteps
                    .Where(s => s.StepOrder < stepOrder)
                    .All(s => s.Status == statusApproved.Id);

                if (!previousStepsApproved)
                    throw new Exception("No se puede aprobar este paso hasta que los pasos anteriores estén aprobados.");

                currentStep.ApproverUserId = userId;
                currentStep.Observations = observations;
                currentStep.DecisionDate = DateTime.Now;
                currentStep.Status = isApproved ? statusApproved.Id : statusRejected.Id;

                await _stepRepo.UpdateAsync(currentStep);

                if (!isApproved)
                {
                    projectProposal.Status = statusRejected.Id;
                    await _proposalRepo.UpdateAsync(projectProposal);
                    await _unitOfWork.CommitAsync();
                    return;
                }

                bool allApproved = allSteps.All(s => s.StepOrder <= stepOrder ? s.Status == statusApproved.Id : true);

                if (allApproved)
                {
                    projectProposal.Status = statusApproved.Id;
                    await _proposalRepo.UpdateAsync(projectProposal);
                }

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }


        public async Task<List<ProjectProposal>> GetProposalsByStatusNameAsync(string statusName)
        {
            var status = await _statusRepo.GetByNameAsync(statusName)
                ?? throw new Exception($"Estado '{statusName}' no encontrado.");

            var allProposals = await _proposalRepo.GetAllAsync();
            return allProposals.Where(p => p.Status == status.Id).ToList();
        }

        public async Task MarkAsObservedAsync(Guid projectId)
        {
            var proposal = await _proposalRepo.GetByIdAsync(projectId)
                ?? throw new Exception("Proyecto no encontrado.");

            proposal.Status = 4;
            await _proposalRepo.UpdateAsync(proposal);

            var steps = await _stepRepo.GetByProposalIdAsync(projectId);

            foreach (var step in steps)
            {
                if (step.Status == 3)
                {
                    step.Status = 4;
                    await _stepRepo.UpdateAsync(step);
                }
            }
        }


    }
}

