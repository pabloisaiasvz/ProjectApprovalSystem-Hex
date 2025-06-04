using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using Application.Services;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProjectApprovalService : IProjectApprovalService
    {
        private readonly IProjectProposalRepository _proposalRepo;
        private readonly IApprovalStatusRepository _statusRepo;
        private readonly IApprovalRuleRepository _ruleRepo;
        private readonly IProjectApprovalStepRepository _stepRepo;
        private readonly IApproverRoleRepository _roleRepo;
        private readonly IUnitOfWork _unitOfWork;


        public ProjectApprovalService(
            IProjectProposalRepository proposalRepo,
            IApprovalStatusRepository statusRepo,
            IApprovalRuleRepository ruleRepo,
            IProjectApprovalStepRepository stepRepo,
            IApproverRoleRepository roleRepo,
            IUnitOfWork unitOfWork)

        {
            _proposalRepo = proposalRepo;
            _statusRepo = statusRepo;
            _ruleRepo = ruleRepo;
            _stepRepo = stepRepo;
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid?> CreateProjectProposalAsync(string title, string description, decimal estimatedAmount, int duration, int areaId, int projectTypeId, int userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var pendingStatus = await _statusRepo.GetByNameAsync("Pending");
                if (pendingStatus == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var projectProposal = new ProjectProposal
                {
                    Title = title,
                    Description = description,
                    EstimatedAmount = estimatedAmount,
                    EstimatedDuration = duration,
                    Area = areaId,
                    Type = projectTypeId,
                    Status = pendingStatus.Id,
                    ApprovalStatus = pendingStatus,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                await _proposalRepo.AddAsync(projectProposal);
                var rules = (await _ruleRepo.GetRulesAsync(estimatedAmount, areaId, projectTypeId)).ToList();

                if (!rules.Any())
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                foreach (var rule in rules)
                {
                    var approverRole = await _roleRepo.GetByIdAsync(rule.ApproverRoleId);
                    var step = new ProjectApprovalStep
                    {
                        ProjectProposalId = projectProposal.Id,
                        ApproverRoleId = rule.ApproverRoleId,
                        Status = pendingStatus.Id,
                        ApprovalStatus = pendingStatus,
                        StepOrder = rule.StepOrder,
                        ApproverRole = approverRole
                    };
                    await _stepRepo.AddAsync(step);
                }

                await _unitOfWork.CommitAsync();

                return projectProposal.Id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return null;
            }
        }

        public async Task ProcessApprovalAsync(Guid projectId, bool isApproved, int stepOrder, int userId, string? observations = null)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var projectProposal = await _proposalRepo.GetByIdAsync(projectId);
                if (projectProposal == null) throw new Exception("Proyecto no encontrado.");

                var currentStep = await _stepRepo.GetByProposalIdAndStepOrderAsync(projectId, stepOrder);
                if (currentStep == null) throw new Exception("Paso de aprobación no encontrado.");

                var statusApproved = await _statusRepo.GetByNameAsync("Approved");
                var statusRejected = await _statusRepo.GetByNameAsync("Rejected");
                if (statusApproved == null || statusRejected == null) throw new Exception("Estados 'Approved' o 'Rejected' no encontrados.");

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

                var allSteps = await _stepRepo.GetByProposalIdAsync(projectId);
                bool allApproved = allSteps.All(s => s.Status == statusApproved.Id);

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

        public async Task UpdateProjectStatusAsync(Guid projectId, int statusId)
        {
            var projectProposal = await _proposalRepo.GetByIdAsync(projectId);
            if (projectProposal == null) throw new Exception("Proyecto no encontrado.");

            var status = await _statusRepo.GetByIdAsync(statusId);
            if (status == null) throw new Exception("Estado no válido.");

            projectProposal.Status = status.Id;
            projectProposal.ApprovalStatus = status;

            await _proposalRepo.UpdateAsync(projectProposal);
        }
    }
}
