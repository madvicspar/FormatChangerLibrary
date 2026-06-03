using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RuleWeightsJson",
                table: "EvaluationSystems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "EvaluationSystems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleWeightsJson",
                table: "EvaluationSystems");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "EvaluationSystems");
        }
    }
}
