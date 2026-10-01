using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IS7012_FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollEntitiesAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayrollPeriods",
                columns: table => new
                {
                    PayrollPeriodId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPeriods", x => x.PayrollPeriodId);
                });

            migrationBuilder.CreateTable(
                name: "SalaryPackages",
                columns: table => new
                {
                    SalaryPackageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "TEXT", nullable: false),
                    Allowance = table.Column<decimal>(type: "TEXT", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryPackages", x => x.SalaryPackageId);
                    table.ForeignKey(
                        name: "FK_SalaryPackages_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRecords",
                columns: table => new
                {
                    PayrollRecordId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PayrollPeriodId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdditionalEarnings = table.Column<decimal>(type: "TEXT", nullable: false),
                    Deductions = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PreparedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    PreparedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRecords", x => x.PayrollRecordId);
                    table.ForeignKey(
                        name: "FK_PayrollRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollRecords_PayrollPeriods_PayrollPeriodId",
                        column: x => x.PayrollPeriodId,
                        principalTable: "PayrollPeriods",
                        principalColumn: "PayrollPeriodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollRecords_SalaryPackages_SalaryPackageId",
                        column: x => x.SalaryPackageId,
                        principalTable: "SalaryPackages",
                        principalColumn: "SalaryPackageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PayrollPeriods",
                columns: new[] { "PayrollPeriodId", "EndDate", "IsClosed", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "SalaryPackages",
                columns: new[] { "SalaryPackageId", "Allowance", "BasicSalary", "EffectiveFrom", "EffectiveTo", "EmployeeId" },
                values: new object[,]
                {
                    { 1, 1000m, 6000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1 },
                    { 2, 1500m, 9000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2 },
                    { 3, 1000m, 6500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3 },
                    { 4, 800m, 5500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 5, 900m, 6000m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4 },
                    { 6, 1500m, 9500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5 },
                    { 7, 900m, 6200m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRecords_EmployeeId_PayrollPeriodId",
                table: "PayrollRecords",
                columns: new[] { "EmployeeId", "PayrollPeriodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRecords_PayrollPeriodId",
                table: "PayrollRecords",
                column: "PayrollPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollRecords_SalaryPackageId",
                table: "PayrollRecords",
                column: "SalaryPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPackages_EmployeeId",
                table: "SalaryPackages",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollRecords");

            migrationBuilder.DropTable(
                name: "PayrollPeriods");

            migrationBuilder.DropTable(
                name: "SalaryPackages");
        }
    }
}
