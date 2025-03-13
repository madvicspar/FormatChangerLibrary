using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class SettingsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormattingTemplates_HeadingSettingsModel_HeadingSettingsId",
                table: "FormattingTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_HeadingSettingsModel_TextSettings_TextSettingsId",
                table: "HeadingSettingsModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HeadingSettingsModel",
                table: "HeadingSettingsModel");

            migrationBuilder.RenameTable(
                name: "HeadingSettingsModel",
                newName: "HeadingSettings");

            migrationBuilder.RenameIndex(
                name: "IX_HeadingSettingsModel_TextSettingsId",
                table: "HeadingSettings",
                newName: "IX_HeadingSettings_TextSettingsId");

            migrationBuilder.AddColumn<long>(
                name: "CaptionSettingsId",
                table: "ImageSettings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FormattingTemplates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeadingSettings",
                table: "HeadingSettings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSettings_CaptionSettingsId",
                table: "ImageSettings",
                column: "CaptionSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormattingTemplates_HeadingSettings_HeadingSettingsId",
                table: "FormattingTemplates",
                column: "HeadingSettingsId",
                principalTable: "HeadingSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HeadingSettings_TextSettings_TextSettingsId",
                table: "HeadingSettings",
                column: "TextSettingsId",
                principalTable: "TextSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageSettings_CaptionSettings_CaptionSettingsId",
                table: "ImageSettings",
                column: "CaptionSettingsId",
                principalTable: "CaptionSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormattingTemplates_HeadingSettings_HeadingSettingsId",
                table: "FormattingTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_HeadingSettings_TextSettings_TextSettingsId",
                table: "HeadingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageSettings_CaptionSettings_CaptionSettingsId",
                table: "ImageSettings");

            migrationBuilder.DropIndex(
                name: "IX_ImageSettings_CaptionSettingsId",
                table: "ImageSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HeadingSettings",
                table: "HeadingSettings");

            migrationBuilder.DropColumn(
                name: "CaptionSettingsId",
                table: "ImageSettings");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FormattingTemplates");

            migrationBuilder.RenameTable(
                name: "HeadingSettings",
                newName: "HeadingSettingsModel");

            migrationBuilder.RenameIndex(
                name: "IX_HeadingSettings_TextSettingsId",
                table: "HeadingSettingsModel",
                newName: "IX_HeadingSettingsModel_TextSettingsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeadingSettingsModel",
                table: "HeadingSettingsModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormattingTemplates_HeadingSettingsModel_HeadingSettingsId",
                table: "FormattingTemplates",
                column: "HeadingSettingsId",
                principalTable: "HeadingSettingsModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HeadingSettingsModel_TextSettings_TextSettingsId",
                table: "HeadingSettingsModel",
                column: "TextSettingsId",
                principalTable: "TextSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
