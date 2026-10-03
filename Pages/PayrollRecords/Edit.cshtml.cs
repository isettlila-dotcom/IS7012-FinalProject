using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollRecords
{
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
                return BadRequest("Only Draft or Returned payroll records can be edited.");
            }

            if (existingRecord.PayrollPeriod == null ||
                existingRecord.PayrollPeriod.IsClosed)
            {
                return BadRequest("Payroll records in a closed payroll period cannot be edited.");
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