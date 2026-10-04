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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            if (!User.HasClaim(
                    "AppRole",
                    "Payroll Team Lead"))
            {
                return Forbid();
            }

            var payrollRecord = await _context.PayrollRecords
                .Include(p => p.PayrollPeriod)
                .FirstOrDefaultAsync(
                    p => p.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                return NotFound();
            }

            if (payrollRecord.Status != PayrollStatus.Submitted)
            {
                TempData["ErrorMessage"] =
                    "Only Submitted payroll records can be approved.";

                return RedirectToPage();
            }

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Records in a closed payroll period cannot be approved.";

                return RedirectToPage();
            }

            var currentUserId =
                User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Forbid();
            }

            if (payrollRecord.PreparedByUserId == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "You cannot approve a payroll record that you prepared.";

                return RedirectToPage();
            }

            payrollRecord.Status =
                PayrollStatus.Approved;

            payrollRecord.ReviewedByUserId =
                currentUserId;

            payrollRecord.ReviewedDate =
                DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReturnAsync(int id)
        {
            if (!User.HasClaim(
                    "AppRole",
                    "Payroll Team Lead"))
            {
                return Forbid();
            }

            var payrollRecord = await _context.PayrollRecords
                .Include(p => p.PayrollPeriod)
                .FirstOrDefaultAsync(
                    p => p.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                return NotFound();
            }

            if (payrollRecord.Status != PayrollStatus.Submitted)
            {
                TempData["ErrorMessage"] =
                    "Only Submitted payroll records can be returned.";

                return RedirectToPage();
            }

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Records in a closed payroll period cannot be returned.";

                return RedirectToPage();
            }

            var currentUserId =
                User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Forbid();
            }

            payrollRecord.Status =
                PayrollStatus.Returned;

            payrollRecord.ReviewedByUserId =
                currentUserId;

            payrollRecord.ReviewedDate =
                DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClosePeriodAsync(
            int payrollPeriodId)
        {
            if (!User.HasClaim(
                    "AppRole",
                    "Payroll Team Lead"))
            {
                return Forbid();
            }

            var period = await _context.PayrollPeriods
                .Include(p => p.PayrollRecords)
                .FirstOrDefaultAsync(
                    p => p.PayrollPeriodId == payrollPeriodId);

            if (period == null)
            {
                return NotFound();
            }

            if (period.IsClosed)
            {
                TempData["ErrorMessage"] =
                    "This payroll period is already closed.";

                return RedirectToPage();
            }

            var hasUnapprovedRecords =
                period.PayrollRecords.Any(r =>
                    r.Status != PayrollStatus.Approved);

            if (hasUnapprovedRecords)
            {
                TempData["ErrorMessage"] =
                    "A payroll period cannot be closed until all payroll records are approved.";

                return RedirectToPage();
            }

            period.IsClosed = true;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}