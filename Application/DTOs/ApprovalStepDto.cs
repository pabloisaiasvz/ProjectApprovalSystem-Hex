using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class ApprovalStepDto
{
    public int Id { get; set; }
    public int ProjectProposalId { get; set; }
    public int StepNumber { get; set; }
    public int ApproverUserId { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
