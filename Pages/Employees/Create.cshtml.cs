using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee()
        {
            HireDate = DateTime.Today,
            IsActive = true
        };

        public SelectList DepartmentList { get; set; } = null!;
        public SelectList JobRoleList { get; set; } = null!;

        public async Task OnGetAsync()
        {
            await LoadDropDownsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync();
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadDropDownsAsync()
        {
            DepartmentList = new SelectList(
                await _context.Departments
                    .OrderBy(d => d.Name)
                    .ToListAsync(),
                "Id",
                "Name");

            JobRoleList = new SelectList(
                await _context.JobRoles
                    .OrderBy(j => j.Title)
                    .ToListAsync(),
                "Id",
                "Title");
        }
    }
}