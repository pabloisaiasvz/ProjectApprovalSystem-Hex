using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProjectProposalSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal EstimatedAmount { get; set; }
        public string AreaName { get; set; }
        public string TypeName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
