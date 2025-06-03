using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class CreateProjectProposalRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int CreatedByUserId { get; set; }
    public List<int> ApproverUserIds { get; set; } = new();
}

