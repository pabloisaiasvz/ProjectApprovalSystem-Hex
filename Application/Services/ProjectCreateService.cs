using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ProjectCreateService : IProjectCreateService
    {
        private readonly IProjectProposalRepository _proposalRepo;
        private readonly IApprovalStatusRepository _statusRepo;
        private readonly IApprovalRuleRepository _ruleRepo;
        private readonly IProjectApprovalStepRepository _stepRepo;
        private readonly IApproverRoleRepository _roleRepo;
        private readonly IUnitOfWork _unitOfWork;


        public ProjectCreateService(
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

                var allRules = (await _ruleRepo.GetRulesAsync(estimatedAmount, areaId, projectTypeId)).ToList();
                var rules = GetMostSpecificUniqueRules(allRules);

                if (!rules.Any())
                {
                    await _unitOfWork.RollbackAsync();
                    return null;
                }

                var orderedRules = rules.OrderBy(r => r.StepOrder).ToList();

                for (int i = 0; i < orderedRules.Count; i++)
                {
                    var rule = orderedRules[i];
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



        private static List<ApprovalRule> GetMostSpecificUniqueRules(List<ApprovalRule> applicableRules)
        {
            var result = new List<ApprovalRule>();

            var groupedByStep = applicableRules.GroupBy(r => r.StepOrder);

            foreach (var stepGroup in groupedByStep)
            {
                var mostSpecific = stepGroup
                    .OrderByDescending(rule => CalculateSpecificity(rule))
                    .ThenBy(rule => rule.Id)
                    .First();

                result.Add(mostSpecific);
            }

            var finalResult = new List<ApprovalRule>();
            var groupedByRole = result.GroupBy(r => r.ApproverRoleId);

            foreach (var roleGroup in groupedByRole)
            {
                if (roleGroup.Count() == 1)
                {
                    finalResult.Add(roleGroup.First());
                }
                else
                {
                    var mostSpecificForRole = roleGroup
                        .OrderByDescending(r => CalculateSpecificity(r))
                        .ThenBy(r => r.StepOrder)
                        .First();

                    finalResult.Add(mostSpecificForRole);
                }
            }

            return finalResult.OrderBy(r => r.StepOrder).ToList();
        }

        private static int CalculateSpecificity(ApprovalRule rule)
        {
            int specificity = 0;

            if (rule.Area.HasValue) specificity++;
            if (rule.Type.HasValue) specificity++;
            if (rule.MinAmount > 0) specificity++;
            if (rule.MaxAmount > 0) specificity++;

            return specificity;
        }
    }
}

