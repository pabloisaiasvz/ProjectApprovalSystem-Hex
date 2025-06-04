using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class ProjectProposalDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal EstimatedAmount { get; set; }
    public int EstimatedDuration { get; set; }
    public int Area { get; set; }
    public string AreaName { get; set; }
    public int Type { get; set; }
    public string TypeName { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

