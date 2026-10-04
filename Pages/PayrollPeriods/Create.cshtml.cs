using System.Globalization;
using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollPeriods
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string PayrollMonth { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var latestPeriod = await _context.PayrollPeriods
                .OrderByDescending(p => p.EndDate)
                .FirstOrDefaultAsync();

            var defaultMonth = latestPeriod != null
                ? latestPeriod.EndDate.AddMonths(1)
                : DateTime.Today;

            PayrollMonth = defaultMonth.ToString("yyyy-MM");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!DateTime.TryParseExact(
                    PayrollMonth,
                    "yyyy-MM",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var selectedMonth))
            {
                ModelState.AddModelError(
                    nameof(PayrollMonth),
                    "Please select a valid payroll month.");

                return Page();
            }

            var startDate = new DateTime(
                selectedMonth.Year,
                selectedMonth.Month,
                1);

            var endDate = new DateTime(
                selectedMonth.Year,
                selectedMonth.Month,
                DateTime.DaysInMonth(
                    selectedMonth.Year,
                    selectedMonth.Month));

            var periodAlreadyExists = await _context.PayrollPeriods
                .AnyAsync(p =>
                    startDate <= p.EndDate &&
                    endDate >= p.StartDate);

            if (periodAlreadyExists)
            {
                ModelState.AddModelError(
                    nameof(PayrollMonth),
                    "A payroll period already exists for this month.");

                return Page();
            }

            var payrollPeriod = new PayrollPeriod
            {
                StartDate = startDate,
                EndDate = endDate,
                IsClosed = false
            };

            _context.PayrollPeriods.Add(payrollPeriod);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"{payrollPeriod.PeriodName} payroll period was created and opened.";

            return RedirectToPage("./Index");
        }
    }
}