using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollRecords
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PayrollRecord> PayrollRecord { get; set; } = new List<PayrollRecord>();

        public async Task OnGetAsync()
        {
            PayrollRecord = await _context.PayrollRecords
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.SalaryPackage)
                .ToListAsync();
        }
    }
}