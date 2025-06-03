namespace Domain.Entities
{
    public class ProjectProposal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }

        public int Area { get; set; }
        public Area? Areas { get; set; }
        public int Type { get; set; }
        public ProjectType ProjectType { get; set; }

        public decimal EstimatedAmount { get; set; }
        public int EstimatedDuration { get; set; }

        public int Status { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }
        public User User { get; set; }

        public ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; }

    }
}
