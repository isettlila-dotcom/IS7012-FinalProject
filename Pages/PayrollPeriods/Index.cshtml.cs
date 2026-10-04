using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollPeriods
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PayrollPeriodSummary> PayrollPeriods { get; set; } = new();

        public async Task OnGetAsync()
        {
            var periods = await _context.PayrollPeriods
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            var expectedEmployees = await _context.Employees
                .CountAsync(e => e.IsActive);

            foreach (var period in periods)
            {
                var records = await _context.PayrollRecords
                    .Where(r => r.PayrollPeriodId == period.PayrollPeriodId)
                    .ToListAsync();

                var approvedCount = records.Count(r =>
                    r.Status == PayrollStatus.Approved);

                PayrollPeriods.Add(new PayrollPeriodSummary
                {
                    PayrollPeriodId = period.PayrollPeriodId,
                    PeriodName = period.PeriodName,
                    ExpectedEmployees = expectedEmployees,
                    RecordCount = records.Count,
                    ApprovedCount = approvedCount,
                    IsClosed = period.IsClosed
                });
            }
        }

        public async Task<IActionResult> OnPostCloseAsync(int id)
        {
            var payrollPeriod = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => p.PayrollPeriodId == id);

            if (payrollPeriod == null)
            {
                TempData["ErrorMessage"] =
                    "Payroll period could not be found.";

                return RedirectToPage();
            }

            if (payrollPeriod.IsClosed)
            {
                TempData["ErrorMessage"] =
                    $"{payrollPeriod.PeriodName} is already closed.";

                return RedirectToPage();
            }

            var expectedEmployees = await _context.Employees
                .CountAsync(e => e.IsActive);

            var recordCount = await _context.PayrollRecords
                .CountAsync(r =>
                    r.PayrollPeriodId == id);

            var approvedCount = await _context.PayrollRecords
                .CountAsync(r =>
                    r.PayrollPeriodId == id &&
                    r.Status == PayrollStatus.Approved);

            if (expectedEmployees == 0 ||
                recordCount != expectedEmployees ||
                approvedCount != expectedEmployees)
            {
                TempData["ErrorMessage"] =
                    $"{payrollPeriod.PeriodName} cannot be closed. " +
                    "All required payroll records must exist and be approved.";

                return RedirectToPage();
            }

            payrollPeriod.IsClosed = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{payrollPeriod.PeriodName} payroll period was successfully closed.";

            return RedirectToPage();
        }

        public class PayrollPeriodSummary
        {
            public int PayrollPeriodId { get; set; }

            public string PeriodName { get; set; } = string.Empty;

            public int ExpectedEmployees { get; set; }

            public int RecordCount { get; set; }

            public int ApprovedCount { get; set; }

            public bool IsClosed { get; set; }

            public int RemainingCount =>
                Math.Max(0, ExpectedEmployees - ApprovedCount);

            public bool CanClose =>
                !IsClosed &&
                ExpectedEmployees > 0 &&
                RecordCount == ExpectedEmployees &&
                ApprovedCount == ExpectedEmployees;
        }
    }
}