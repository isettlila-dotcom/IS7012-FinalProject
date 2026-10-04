using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollRecords


{
    [Authorize(Policy = "PayrrollSpecialist")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            if (payrollRecord.Status != PayrollStatus.Draft &&
                payrollRecord.Status != PayrollStatus.Returned)
            {
                TempData["ErrorMessage"] =
                    "This payroll record cannot be edited because it has already been submitted for review.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PayrollPeriod == null ||
                payrollRecord.PayrollPeriod.IsClosed)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed payroll period cannot be edited.";

                return RedirectToPage("./Index");
            }
            PayrollRecord = payrollRecord;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var existingRecord = await _context.PayrollRecords
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.SalaryPackage)
                .FirstOrDefaultAsync(
                    p => p.PayrollRecordId == PayrollRecord.PayrollRecordId);

            if (existingRecord == null)
            {
                return NotFound();
            }

            if (existingRecord.Status != PayrollStatus.Draft &&
                existingRecord.Status != PayrollStatus.Returned)
            {
                TempData["ErrorMessage"] =
                    "This payroll record cannot be edited because it has already been submitted for review.";

                return RedirectToPage("./Index");
            }

            if (existingRecord.PayrollPeriod == null ||
                existingRecord.PayrollPeriod.IsClosed)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed payroll period cannot be edited.";

                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                PayrollRecord = existingRecord;
                return Page();
            }

            var grossPay =
                existingRecord.SalaryPackage!.BasicSalary
                + existingRecord.SalaryPackage.Allowance
                + PayrollRecord.AdditionalEarnings;

            if (PayrollRecord.Deductions > grossPay)
            {
                ModelState.AddModelError(
                    "PayrollRecord.Deductions",
                    "Deductions cannot exceed gross pay.");

                existingRecord.AdditionalEarnings = PayrollRecord.AdditionalEarnings;
                existingRecord.Deductions = PayrollRecord.Deductions;
                existingRecord.Remarks = PayrollRecord.Remarks;

                PayrollRecord = existingRecord;

                return Page();
            }

            existingRecord.AdditionalEarnings = PayrollRecord.AdditionalEarnings;
            existingRecord.Deductions = PayrollRecord.Deductions;
            existingRecord.Remarks = PayrollRecord.Remarks;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}