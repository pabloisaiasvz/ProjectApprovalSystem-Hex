using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class ApprovalDecisionRequest
{
    public int ProposalId { get; set; }
    public int ApproverUserId { get; set; }
    public bool IsApproved { get; set; }
}

