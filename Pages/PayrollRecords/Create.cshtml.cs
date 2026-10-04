using IS7012_FinalProject.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IS7012_FinalProject.Models;

namespace IS7012_FinalProject.Pages.PayrollRecords
{
    [Authorize(Policy = "PayrrollSpecialist")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Payroll Period")]
        public int PayrollPeriodId { get; set; }

        [BindProperty]
        [Range(0, double.MaxValue, ErrorMessage = "Additional earnings cannot be negative.")]
        [Display(Name = "Additional Earnings")]
        public decimal AdditionalEarnings { get; set; }

        [BindProperty]
        [Range(0, double.MaxValue, ErrorMessage = "Deductions cannot be negative.")]
        public decimal Deductions { get; set; }

        [BindProperty]
        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<SelectListItem> EmployeeOptions { get; set; } = new();
        public List<SelectListItem> PayrollPeriodOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDropdownsAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == EmployeeId);

            if (employee == null || !employee.IsActive)
            {
                ModelState.AddModelError("EmployeeId", "Please select a valid active employee.");
                await LoadDropdownsAsync();
                return Page();
            }

            var payrollPeriod = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => p.PayrollPeriodId == PayrollPeriodId);

            if (payrollPeriod == null)
            {
                ModelState.AddModelError("PayrollPeriodId", "Please select a valid payroll period.");
                await LoadDropdownsAsync();
                return Page();
            }

            if (payrollPeriod.IsClosed)
            {
                ModelState.AddModelError("PayrollPeriodId", "Payroll records cannot be created for a closed payroll period.");
                await LoadDropdownsAsync();
                return Page();
            }

            var recordExists = await _context.PayrollRecords
                .AnyAsync(p => p.EmployeeId == EmployeeId
                            && p.PayrollPeriodId == PayrollPeriodId);

            if (recordExists)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A payroll record already exists for this employee and payroll period.");

                await LoadDropdownsAsync();
                return Page();
            }

            var salaryPackage = await GetSalaryPackageAsync(EmployeeId, payrollPeriod);

            if (salaryPackage == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No salary package was found for this employee and payroll period.");

                await LoadDropdownsAsync();
                return Page();
            }
            var grossPay = salaryPackage.BasicSalary
                + salaryPackage.Allowance
                + AdditionalEarnings;

            if (Deductions > grossPay)
            {
                ModelState.AddModelError(
                    "Deductions",
                    "Deductions cannot exceed gross pay.");

                await LoadDropdownsAsync();
                return Page();
            }
            var payrollRecord = new PayrollRecord
            {
                EmployeeId = EmployeeId,
                PayrollPeriodId = PayrollPeriodId,
                SalaryPackageId = salaryPackage.SalaryPackageId,
                AdditionalEarnings = AdditionalEarnings,
                Deductions = Deductions,
                Remarks = Remarks,
                Status = PayrollStatus.Draft
            };

            _context.PayrollRecords.Add(payrollRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadDropdownsAsync()
        {
            EmployeeOptions = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.EmployeeNumber + " - " + e.FirstName + " " + e.LastName
                })
                .ToListAsync();

            var payrollPeriods = await _context.PayrollPeriods
                .Where(p => !p.IsClosed)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            PayrollPeriodOptions = payrollPeriods
                .Select(p => new SelectListItem
                {
                    Value = p.PayrollPeriodId.ToString(),
                    Text = p.StartDate.ToString("MMMM yyyy")
                })
                .ToList();
        }

        private async Task<SalaryPackage?> GetSalaryPackageAsync(
            int employeeId,
            PayrollPeriod payrollPeriod)
        {
            return await _context.SalaryPackages
                .Where(s => s.EmployeeId == employeeId)
                .Where(s => s.EffectiveFrom <= payrollPeriod.StartDate)
                .Where(s => s.EffectiveTo == null || s.EffectiveTo >= payrollPeriod.StartDate)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync();
        }
    }
}