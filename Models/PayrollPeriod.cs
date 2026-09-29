using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IS7012_FinalProject.Models
{
    public class PayrollPeriod
    {
        public int PayrollPeriodId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Closed")]
        public bool IsClosed { get; set; }

        [NotMapped]
        [Display(Name = "Payroll Period")]
        public string PeriodName => StartDate.ToString("MMMM yyyy");

        public List<PayrollRecord> PayrollRecords { get; set; } = new();
    }
}