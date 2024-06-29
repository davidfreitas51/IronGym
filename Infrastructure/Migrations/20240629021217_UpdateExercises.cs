using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropColumn(
                name: "WeekDay",
                table: "ExerciseDay");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExerciseDay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ExerciseDay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "ExerciseDay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExerciseDay");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ExerciseDay");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "ExerciseDay");

            migrationBuilder.AddColumn<int>(
                name: "WeekDay",
                table: "ExerciseDay",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExerciseDayId = table.Column<int>(type: "int", nullable: true),
                    ExerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercise_ExerciseDay_ExerciseDayId",
                        column: x => x.ExerciseDayId,
                        principalTable: "ExerciseDay",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_ExerciseDayId",
                table: "Exercise",
                column: "ExerciseDayId");
        }
    }
}
