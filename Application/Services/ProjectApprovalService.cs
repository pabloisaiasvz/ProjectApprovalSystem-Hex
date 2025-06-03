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

        public ProjectApprovalService(
            IProjectProposalRepository proposalRepo,
            IApprovalStatusRepository statusRepo,
            IApprovalRuleRepository ruleRepo,
            IProjectApprovalStepRepository stepRepo,
            IApproverRoleRepository roleRepo)
        {
            _proposalRepo = proposalRepo;
            _statusRepo = statusRepo;
            _ruleRepo = ruleRepo;
            _stepRepo = stepRepo;
            _roleRepo = roleRepo;
        }

        public async Task<Guid?> CreateProjectProposalAsync(string title, string description, decimal estimatedAmount, int duration, int areaId, int projectTypeId, int userId)
        {
            var pendingStatus = await _statusRepo.GetByNameAsync("Pending");
            if (pendingStatus == null) return null;

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

            await _proposalRepo.SaveChangesAsync();

            var rules = (await _ruleRepo.GetRulesAsync(estimatedAmount, areaId, projectTypeId)).ToList();
            if (!rules.Any()) return null;

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

            await _stepRepo.SaveChangesAsync();

            return projectProposal.Id;
        }

        public async Task ProcessApprovalAsync(Guid projectId, bool isApproved, int stepOrder, int userId, string? observations = null)
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
                return;
            }

            var allSteps = await _stepRepo.GetByProposalIdAsync(projectId);
            bool allApproved = allSteps.All(s => s.Status == statusApproved.Id);

            if (allApproved)
            {
                projectProposal.Status = statusApproved.Id;
                await _proposalRepo.UpdateAsync(projectProposal);
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
