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
    }
}