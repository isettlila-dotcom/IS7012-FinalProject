using Microsoft.EntityFrameworkCore;
using IS7012_FinalProject.Models;

namespace IS7012_FinalProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<JobRole> JobRoles { get; set; }
        public DbSet<SalaryPackage> SalaryPackages { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<PayrollRecord> PayrollRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Accounting",
                    Location = "Main Office"
                },
                new Department
                {
                    Id = 2,
                    Name = "Human Resources",
                    Location = "Main Office"
                },
                new Department
                {
                    Id = 3,
                    Name = "Information Technology",
                    Location = "Main Office"
                }
            );

            modelBuilder.Entity<JobRole>().HasData(
                new JobRole
                {
                    Id = 1,
                    Title = "Payroll Specialist",
                    Description = "Processes employee payroll"
                },
                new JobRole
                {
                    Id = 2,
                    Title = "Payroll Team Lead",
                    Description = "Reviews and approves payroll"
                },
                new JobRole
                {
                    Id = 3,
                    Title = "HR Specialist",
                    Description = "Supports employee administration"
                }

            );
            // SalaryPackage belongs to one Employee
            modelBuilder.Entity<SalaryPackage>()
                .HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollRecord belongs to one Employee
            modelBuilder.Entity<PayrollRecord>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollRecord belongs to one PayrollPeriod
            modelBuilder.Entity<PayrollRecord>()
                .HasOne(p => p.PayrollPeriod)
                .WithMany(pp => pp.PayrollRecords)
                .HasForeignKey(p => p.PayrollPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            // PayrollRecord belongs to one SalaryPackage
            modelBuilder.Entity<PayrollRecord>()
                .HasOne(p => p.SalaryPackage)
                .WithMany(s => s.PayrollRecords)
                .HasForeignKey(p => p.SalaryPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // One payroll record per employee per payroll period
            modelBuilder.Entity<PayrollRecord>()
                .HasIndex(p => new { p.EmployeeId, p.PayrollPeriodId })
                .IsUnique();
        }
    }
}