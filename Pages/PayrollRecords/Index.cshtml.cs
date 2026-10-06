using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IList<PayrollRecord> PayrollRecord { get; set; }
            = new List<PayrollRecord>();

        public async Task OnGetAsync()
        {
            PayrollRecord = await _context.PayrollRecords
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.SalaryPackage)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostSubmitAsync(int id)
        {
            if (!User.HasClaim(
                    "AppRole",
                    "Payroll Specialist"
                    ))
                {
                    return Forbid();
                }
            var payrollRecord = await _context.PayrollRecords
                .Include(p => p.PayrollPeriod)
                .FirstOrDefaultAsync(p => p.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                return NotFound();
            }

            if (payrollRecord.Status != PayrollStatus.Draft &&
                payrollRecord.Status != PayrollStatus.Returned)
            {
                TempData["ErrorMessage"] =
                    "Only Draft or Returned payroll records can be submitted.";

                return RedirectToPage();
            }

            if (payrollRecord.PayrollPeriod == null ||
                payrollRecord.PayrollPeriod.IsClosed)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed payroll period cannot be submitted.";

                return RedirectToPage();
            }

            var clerkUserId = User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(clerkUserId))
            {
                TempData["ErrorMessage"] =
                    "You must be signed in to submit a payroll record.";

                return RedirectToPage();
            }

            payrollRecord.Status = PayrollStatus.Submitted;
            payrollRecord.PreparedByUserId = clerkUserId;
            payrollRecord.PreparedDate = DateTime.Now;

            // Clear the previous review information when
            // a returned record is resubmitted.
            payrollRecord.ReviewedByUserId = null;
            payrollRecord.ReviewedDate = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Payroll record submitted for review.";

            return RedirectToPage();
        }

    }
}