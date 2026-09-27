namespace IS7012_FinalProject.Models
{
    public class JobRole
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();
    }
}