using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IS7012_FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "EmployeeNumber", "FirstName", "HireDate", "IsActive", "JobRoleId", "LastName" },
                values: new object[,]
                {
                    { 1, 1, "john.smith@company.com", "EMP001", "John", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1, "Smith" },
                    { 2, 1, "sarah.johnson@company.com", "EMP002", "Sarah", new DateTime(2021, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Johnson" },
                    { 3, 2, "michael.brown@company.com", "EMP003", "Michael", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 3, "Brown" },
                    { 4, 1, "emily.davis@company.com", "EMP004", "Emily", new DateTime(2022, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1, "Davis" },
                    { 5, 1, "david.wilson@company.com", "EMP005", "David", new DateTime(2020, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Wilson" },
                    { 6, 2, "jessica.taylor@company.com", "EMP006", "Jessica", new DateTime(2023, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 3, "Taylor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
