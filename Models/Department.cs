namespace IS7012_FinalProject.Models
{
	public class Department
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Location { get; set; } = string.Empty;

		public List<Employee> Employees { get; set; } = new();
	}
}
