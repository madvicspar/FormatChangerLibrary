using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class HeadingLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "HeadingLevel",
                table: "HeadingSettings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "NextHeadingLevelId",
                table: "HeadingSettings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CaptionSettings",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_HeadingSettings_NextHeadingLevelId",
                table: "HeadingSettings",
                column: "NextHeadingLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_HeadingSettings_HeadingSettings_NextHeadingLevelId",
                table: "HeadingSettings",
                column: "NextHeadingLevelId",
                principalTable: "HeadingSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HeadingSettings_HeadingSettings_NextHeadingLevelId",
                table: "HeadingSettings");

            migrationBuilder.DropIndex(
                name: "IX_HeadingSettings_NextHeadingLevelId",
                table: "HeadingSettings");

            migrationBuilder.DropColumn(
                name: "NextHeadingLevelId",
                table: "HeadingSettings");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CaptionSettings");

            migrationBuilder.AlterColumn<int>(
                name: "HeadingLevel",
                table: "HeadingSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
