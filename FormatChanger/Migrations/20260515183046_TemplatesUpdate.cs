using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class TemplatesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndType",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "IsNumeric",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "ListLevel",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "MarkerType",
                table: "ListSettings");

            migrationBuilder.AddColumn<long>(
                name: "BulletListSettingsId",
                table: "ListSettings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NumberedListSettingsId",
                table: "ListSettings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HeadingLevel",
                table: "HeadingSettings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "CaptionSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Separator",
                table: "CaptionSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BulletListSettingsModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EndType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BulletListSettingsModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NumberedListSettingsModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level1MarkerType = table.Column<int>(type: "integer", nullable: false),
                    Level1EndType = table.Column<int>(type: "integer", nullable: false),
                    Level2EndType = table.Column<int>(type: "integer", nullable: false),
                    Level3EndType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberedListSettingsModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListSettings_BulletListSettingsId",
                table: "ListSettings",
                column: "BulletListSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ListSettings_NumberedListSettingsId",
                table: "ListSettings",
                column: "NumberedListSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListSettings_BulletListSettingsModel_BulletListSettingsId",
                table: "ListSettings",
                column: "BulletListSettingsId",
                principalTable: "BulletListSettingsModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListSettings_NumberedListSettingsModel_NumberedListSettings~",
                table: "ListSettings",
                column: "NumberedListSettingsId",
                principalTable: "NumberedListSettingsModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_BulletListSettingsModel_BulletListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_NumberedListSettingsModel_NumberedListSettings~",
                table: "ListSettings");

            migrationBuilder.DropTable(
                name: "BulletListSettingsModel");

            migrationBuilder.DropTable(
                name: "NumberedListSettingsModel");

            migrationBuilder.DropIndex(
                name: "IX_ListSettings_BulletListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropIndex(
                name: "IX_ListSettings_NumberedListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "BulletListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "NumberedListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "CaptionSettings");

            migrationBuilder.DropColumn(
                name: "Separator",
                table: "CaptionSettings");

            migrationBuilder.AddColumn<int>(
                name: "EndType",
                table: "ListSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsNumeric",
                table: "ListSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ListLevel",
                table: "ListSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MarkerType",
                table: "ListSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "HeadingLevel",
                table: "HeadingSettings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
