using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollRecords
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PayrollRecord> PayrollRecords { get; set; }
            = new List<PayrollRecord>();

        public IList<PayrollPeriod> PayrollPeriods { get; set; }
            = new List<PayrollPeriod>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PeriodId { get; set; }

        [BindProperty(SupportsGet = true)]
        public PayrollStatus? StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            PayrollPeriods = await _context.PayrollPeriods
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            var query = _context.PayrollRecords
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.SalaryPackage)
                .AsQueryable();

            // Search by employee number, first name or last name
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var search = SearchTerm.Trim();

                query = query.Where(p =>
                    p.Employee != null &&
                    (
                        p.Employee.EmployeeNumber.Contains(search) ||
                        p.Employee.FirstName.Contains(search) ||
                        p.Employee.LastName.Contains(search)
                    ));
            }

            // Filter by payroll period
            if (PeriodId.HasValue)
            {
                query = query.Where(p =>
                    p.PayrollPeriodId == PeriodId.Value);
            }

            // Filter by payroll status
            if (StatusFilter.HasValue)
            {
                query = query.Where(p =>
                    p.Status == StatusFilter.Value);
            }

            PayrollRecords = await query
                .OrderByDescending(p => p.PayrollPeriod!.StartDate)
                .ThenBy(p => p.Employee!.LastName)
                .ThenBy(p => p.Employee!.FirstName)
                .ToListAsync();
        }
    }
}