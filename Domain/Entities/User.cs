namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public int Role { get; set; }
        public ApproverRole ApproverRole { get; set; }

        public ICollection<ProjectProposal> ProjectProposals { get; set; }
        public ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; }

    }
}
