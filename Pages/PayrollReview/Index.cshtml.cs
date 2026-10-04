using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollReview
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PayrollRecord> PayrollRecords { get; set; }
            = new List<PayrollRecord>();

        public async Task OnGetAsync()
        {
            PayrollRecords = await _context.PayrollRecords
                .Include(r => r.Employee)
                .Include(r => r.PayrollPeriod)
                .Include(r => r.SalaryPackage)
                .Where(r => r.Status == PayrollStatus.Submitted)
                .OrderBy(r => r.PayrollPeriod.StartDate)
                .ThenBy(r => r.Employee.LastName)
                .ThenBy(r => r.Employee.FirstName)
                .ToListAsync();
        }
    }
}