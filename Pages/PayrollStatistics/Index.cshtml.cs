using IS7012_FinalProject.Data;
using IS7012_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Pages.PayrollStatistics
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PayrollPeriod> PayrollPeriods { get; set; }
            = new List<PayrollPeriod>();

        [BindProperty(SupportsGet = true)]
        public int? PeriodId { get; set; }

        public int TotalRecords { get; set; }
        public int DraftCount { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int ReturnedCount { get; set; }

        public decimal TotalGrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetPay { get; set; }

        public string SelectedPeriodName { get; set; } = "All Periods";

        public async Task OnGetAsync()
        {
            PayrollPeriods = await _context.PayrollPeriods
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            var query = _context.PayrollRecords
                .Include(p => p.SalaryPackage)
                .Include(p => p.PayrollPeriod)
                .AsQueryable();

            if (PeriodId.HasValue)
            {
                query = query.Where(p =>
                    p.PayrollPeriodId == PeriodId.Value);

                var selectedPeriod = PayrollPeriods
                    .FirstOrDefault(p =>
                        p.PayrollPeriodId == PeriodId.Value);

                if (selectedPeriod != null)
                {
                    SelectedPeriodName = selectedPeriod.PeriodName;
                }
            }

            var records = await query.ToListAsync();

            TotalRecords = records.Count;

            DraftCount = records.Count(p =>
                p.Status == PayrollStatus.Draft);

            SubmittedCount = records.Count(p =>
                p.Status == PayrollStatus.Submitted);

            ApprovedCount = records.Count(p =>
                p.Status == PayrollStatus.Approved);

            ReturnedCount = records.Count(p =>
                p.Status == PayrollStatus.Returned);

            TotalGrossPay = records.Sum(p => p.GrossPay);

            TotalDeductions = records.Sum(p => p.Deductions);

            TotalNetPay = records.Sum(p => p.NetPay);
        }
    }
}