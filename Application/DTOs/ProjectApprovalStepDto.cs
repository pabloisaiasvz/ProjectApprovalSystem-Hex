using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProjectApprovalStepDto
    {
        public long Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public int StepOrder { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime? DecisionDate { get; set; }
        public int? ApprovedBy { get; set; }
        public string? Observations { get; set; }
    }
}
