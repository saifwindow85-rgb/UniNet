using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class SyncSeedGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "StudentResults",
                keyColumn: "StudentResultId",
                keyValue: 1,
                columns: new[] { "Final", "Midterm", "Practical" },
                values: new object[] { 45m, 27m, 16m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "StudentResults",
                keyColumn: "StudentResultId",
                keyValue: 1,
                columns: new[] { "Final", "Midterm", "Practical" },
                values: new object[] { 40m, 45m, 10m });
        }
    }
}
