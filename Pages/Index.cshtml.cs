using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IS7012_FinalProject.Data;

namespace IS7012_FinalProject.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public string EmployeeFirstName { get; set; } = "User";

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var email = User.FindFirst("email")?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e =>
                e.Email.ToLower() == email.ToLower());

        if (employee != null)
        {
            EmployeeFirstName = employee.FirstName;
        }
    }
}