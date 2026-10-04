using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollReview
{
    public class ReviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReviewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PayrollRecord PayrollRecord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var payrollRecord = await _context.PayrollRecords
                .Include(r => r.Employee)
                .Include(r => r.PayrollPeriod)
                .Include(r => r.SalaryPackage)
                .FirstOrDefaultAsync(r => r.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                TempData["ErrorMessage"] =
                    "Payroll record could not be found.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.Status != PayrollStatus.Submitted)
            {
                TempData["ErrorMessage"] =
                    "Only submitted payroll records can be reviewed.";

                return RedirectToPage("./Index");
            }

            PayrollRecord = payrollRecord;

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var payrollRecord = await _context.PayrollRecords
                .Include(r => r.Employee)
                .Include(r => r.PayrollPeriod)
                .FirstOrDefaultAsync(r => r.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                TempData["ErrorMessage"] =
                    "Payroll record could not be found.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.Status != PayrollStatus.Submitted)
            {
                TempData["ErrorMessage"] =
                    "Only submitted payroll records can be approved.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed period cannot be approved.";

                return RedirectToPage("./Index");
            }

            var currentUserId = User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] =
                    "You must be signed in to approve a payroll record.";

                return RedirectToPage("./Index");
            }

            if (string.IsNullOrEmpty(payrollRecord.PreparedByUserId))
            {
                TempData["ErrorMessage"] =
                    "This payroll record cannot be approved because preparer information is missing.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PreparedByUserId == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "You cannot approve a payroll record that you prepared.";

                return RedirectToPage("./Index");
            }

            payrollRecord.Status = PayrollStatus.Approved;
            payrollRecord.ReviewedByUserId = currentUserId;
            payrollRecord.ReviewedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{payrollRecord.Employee?.FirstName} " +
                $"{payrollRecord.Employee?.LastName} - " +
                $"{payrollRecord.PayrollPeriod?.PeriodName} was approved.";

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostReturnAsync(int id)
        {
            var payrollRecord = await _context.PayrollRecords
                .Include(r => r.Employee)
                .Include(r => r.PayrollPeriod)
                .FirstOrDefaultAsync(r => r.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                TempData["ErrorMessage"] =
                    "Payroll record could not be found.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.Status != PayrollStatus.Submitted)
            {
                TempData["ErrorMessage"] =
                    "Only submitted payroll records can be returned.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed period cannot be returned.";

                return RedirectToPage("./Index");
            }

            var currentUserId = User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["ErrorMessage"] =
                    "You must be signed in to return a payroll record.";

                return RedirectToPage("./Index");
            }

            if (string.IsNullOrEmpty(payrollRecord.PreparedByUserId))
            {
                TempData["ErrorMessage"] =
                    "This payroll record cannot be returned because preparer information is missing.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PreparedByUserId == currentUserId)
            {
                TempData["ErrorMessage"] =
                    "You cannot return a payroll record that you prepared.";

                return RedirectToPage("./Index");
            }

            payrollRecord.Status = PayrollStatus.Returned;
            payrollRecord.ReviewedByUserId = currentUserId;
            payrollRecord.ReviewedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{payrollRecord.Employee?.FirstName} " +
                $"{payrollRecord.Employee?.LastName} - " +
                $"{payrollRecord.PayrollPeriod?.PeriodName} was returned for correction.";

            return RedirectToPage("./Index");
        }
    }
}