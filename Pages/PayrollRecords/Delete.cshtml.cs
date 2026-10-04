using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IS7012_FinalProject.Pages.PayrollRecords
{

    [Authorize(Policy = "PayrollSpecialist")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
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

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed period cannot be deleted.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.Status != PayrollStatus.Draft &&
                payrollRecord.Status != PayrollStatus.Returned)
            {
                TempData["ErrorMessage"] =
                    "Only draft or returned payroll records can be deleted.";

                return RedirectToPage("./Index");
            }

            PayrollRecord = payrollRecord;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var payrollRecord = await _context.PayrollRecords
                .Include(r => r.PayrollPeriod)
                .FirstOrDefaultAsync(r => r.PayrollRecordId == id);

            if (payrollRecord == null)
            {
                TempData["ErrorMessage"] =
                    "Payroll record could not be found.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.PayrollPeriod?.IsClosed == true)
            {
                TempData["ErrorMessage"] =
                    "Payroll records in a closed period cannot be deleted.";

                return RedirectToPage("./Index");
            }

            if (payrollRecord.Status != PayrollStatus.Draft &&
                payrollRecord.Status != PayrollStatus.Returned)
            {
                TempData["ErrorMessage"] =
                    "Only draft or returned payroll records can be deleted.";

                return RedirectToPage("./Index");
            }

            _context.PayrollRecords.Remove(payrollRecord);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Payroll record was deleted successfully.";

            return RedirectToPage("./Index");
        }
    }
}