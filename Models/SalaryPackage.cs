using System.ComponentModel.DataAnnotations;

namespace IS7012_FinalProject.Models
{
    public class SalaryPackage
    {
        public int SalaryPackageId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Basic salary cannot be negative.")]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Allowance cannot be negative.")]
        public decimal Allowance { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Effective From")]
        public DateTime EffectiveFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Effective To")]
        public DateTime? EffectiveTo { get; set; }

        public Employee? Employee { get; set; }

        public List<PayrollRecord> PayrollRecords { get; set; } = new();
    }
}