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

            // Department seed data
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

            // Job Role seed data
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

            // Employee seed data
            // Fixed IDs 1-6 are used so SalaryPackage seed data
            // can reference these employees.
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    EmployeeNumber = "EMP001",
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@company.com",
                    HireDate = new DateTime(2022, 1, 10),
                    IsActive = true,
                    DepartmentId = 1,
                    JobRoleId = 1
                },
                new Employee
                {
                    Id = 2,
                    EmployeeNumber = "EMP002",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.johnson@company.com",
                    HireDate = new DateTime(2021, 6, 15),
                    IsActive = true,
                    DepartmentId = 1,
                    JobRoleId = 2
                },
                new Employee
                {
                    Id = 3,
                    EmployeeNumber = "EMP003",
                    FirstName = "Michael",
                    LastName = "Brown",
                    Email = "michael.brown@company.com",
                    HireDate = new DateTime(2023, 3, 20),
                    IsActive = true,
                    DepartmentId = 2,
                    JobRoleId = 3
                },
                new Employee
                {
                    Id = 4,
                    EmployeeNumber = "EMP004",
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@company.com",
                    HireDate = new DateTime(2022, 8, 5),
                    IsActive = true,
                    DepartmentId = 1,
                    JobRoleId = 1
                },
                new Employee
                {
                    Id = 5,
                    EmployeeNumber = "EMP005",
                    FirstName = "David",
                    LastName = "Wilson",
                    Email = "david.wilson@company.com",
                    HireDate = new DateTime(2020, 11, 12),
                    IsActive = true,
                    DepartmentId = 1,
                    JobRoleId = 2
                },
                new Employee
                {
                    Id = 6,
                    EmployeeNumber = "EMP006",
                    FirstName = "Jessica",
                    LastName = "Taylor",
                    Email = "jessica.taylor@company.com",
                    HireDate = new DateTime(2023, 7, 17),
                    IsActive = true,
                    DepartmentId = 2,
                    JobRoleId = 3
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