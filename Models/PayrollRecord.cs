using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS7012_FinalProject.Models
{
    public class PayrollRecord
    {
        public int PayrollRecordId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Payroll Period")]
        public int PayrollPeriodId { get; set; }

        [Required]
        [Display(Name = "Salary Package")]
        public int SalaryPackageId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Additional earnings cannot be negative.")]
        [Display(Name = "Additional Earnings")]
        public decimal AdditionalEarnings { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deductions cannot be negative.")]
        public decimal Deductions { get; set; }

        [Required]
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        [Display(Name = "Prepared By")]
        public string? PreparedByUserId { get; set; }

        [Display(Name = "Prepared Date")]
        public DateTime? PreparedDate { get; set; }

        [Display(Name = "Reviewed By")]
        public string? ReviewedByUserId { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime? ReviewedDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public Employee? Employee { get; set; }

        public PayrollPeriod? PayrollPeriod { get; set; }

        public SalaryPackage? SalaryPackage { get; set; }

        [NotMapped]
        [Display(Name = "Gross Pay")]
        public decimal GrossPay =>
            (SalaryPackage?.BasicSalary ?? 0)
            + (SalaryPackage?.Allowance ?? 0)
            + AdditionalEarnings;

        [NotMapped]
        [Display(Name = "Net Pay")]
        public decimal NetPay => GrossPay - Deductions;
    }
}