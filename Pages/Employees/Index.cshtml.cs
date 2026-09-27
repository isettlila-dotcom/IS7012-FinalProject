using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IS7012_FinalProject.Pages.Employees
{
	public class IndexModel : PageModel
	{
		public List<Employee> Employees { get; set; } = new();

		public void OnGet()
		{
			// Temporary sample data until the database is connected.
			Employees = new List<Employee>
			{
				new Employee
				{
					Id = 1,
					EmployeeNumber = "EMP001",
					FirstName = "John",
					LastName = "Smith",
					Email = "john.smith@example.com",
					HireDate = DateTime.Today,
					IsActive = true,
					Department = new Department
					{
						Name = "Accounting"
					},
					JobRole = new JobRole
					{
						Title = "Payroll Specialist"
					}
				}
			};
		}
	}
}
