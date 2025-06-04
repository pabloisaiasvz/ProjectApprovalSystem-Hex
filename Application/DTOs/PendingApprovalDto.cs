using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PendingApprovalDto
    {
        public long StepId { get; set; }
        public Guid ProjectProposalId { get; set; }
        public string ProjectTitle { get; set; }
        public decimal EstimatedAmount { get; set; }
        public int StepOrder { get; set; }
        public string ApproverRoleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
