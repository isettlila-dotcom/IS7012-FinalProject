using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IS7012_FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedDepartmentsAndJobRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Location", "Name" },
                values: new object[,]
                {
                    { 1, "Main Office", "Accounting" },
                    { 2, "Main Office", "Human Resources" },
                    { 3, "Main Office", "Information Technology" }
                });

            migrationBuilder.InsertData(
                table: "JobRoles",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { 1, "Processes employee payroll", "Payroll Specialist" },
                    { 2, "Reviews and approves payroll", "Payroll Team Lead" },
                    { 3, "Supports employee administration", "HR Specialist" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "JobRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JobRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobRoles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
