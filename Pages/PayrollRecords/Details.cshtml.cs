using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollRecords
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PayrollRecord PayrollRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollRecord = await _context.PayrollRecords
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.SalaryPackage)
                .FirstOrDefaultAsync(p => p.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                return NotFound();
            }

            PayrollRecord = payrollRecord;

            return Page();
        }
    }
}