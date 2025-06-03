namespace Domain.Entities
{
    public class ApproverRole
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ApprovalRule> ApprovalRules { get; set; }
        public ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; }
        public ICollection<User> User { get; set; }
    }
}
