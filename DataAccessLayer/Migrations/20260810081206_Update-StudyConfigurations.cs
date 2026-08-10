using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudyConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentResults_Users_EnteredByUserId",
                table: "StudentResults");

            migrationBuilder.DropIndex(
                name: "IX_StudentResults_StudentId",
                table: "StudentResults");

            migrationBuilder.DropIndex(
                name: "IX_SectionSubjects_SectionId",
                table: "SectionSubjects");

            migrationBuilder.RenameColumn(
                name: "EnteredByUserId",
                table: "StudentResults",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentResults_EnteredByUserId",
                table: "StudentResults",
                newName: "IX_StudentResults_CreatedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "UniversityId",
                table: "Semesters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Semesters",
                keyColumn: "SemesterId",
                keyValue: 1,
                column: "UniversityId",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentResults_StudentId_SectionSubjectId",
                table: "StudentResults",
                columns: new[] { "StudentId", "SectionSubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_UniversityId",
                table: "Semesters",
                column: "UniversityId",
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_UniversityId_Name",
                table: "Semesters",
                columns: new[] { "UniversityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionSubjects_SectionId_SubjectId_SemesterId",
                table: "SectionSubjects",
                columns: new[] { "SectionId", "SubjectId", "SemesterId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Semesters_Universities_UniversityId",
                table: "Semesters",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResults_Users_CreatedByUserId",
                table: "StudentResults",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Semesters_Universities_UniversityId",
                table: "Semesters");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResults_Users_CreatedByUserId",
                table: "StudentResults");

            migrationBuilder.DropIndex(
                name: "IX_StudentResults_StudentId_SectionSubjectId",
                table: "StudentResults");

            migrationBuilder.DropIndex(
                name: "IX_Semesters_UniversityId",
                table: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Semesters_UniversityId_Name",
                table: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_SectionSubjects_SectionId_SubjectId_SemesterId",
                table: "SectionSubjects");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Semesters");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "StudentResults",
                newName: "EnteredByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentResults_CreatedByUserId",
                table: "StudentResults",
                newName: "IX_StudentResults_EnteredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentResults_StudentId",
                table: "StudentResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionSubjects_SectionId",
                table: "SectionSubjects",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResults_Users_EnteredByUserId",
                table: "StudentResults",
                column: "EnteredByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
