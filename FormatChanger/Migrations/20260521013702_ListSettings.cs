using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class ListSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_BulletListSettingsModel_BulletListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_NumberedListSettingsModel_NumberedListSettings~",
                table: "ListSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NumberedListSettingsModel",
                table: "NumberedListSettingsModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BulletListSettingsModel",
                table: "BulletListSettingsModel");

            migrationBuilder.RenameTable(
                name: "NumberedListSettingsModel",
                newName: "NumberedListSettings");

            migrationBuilder.RenameTable(
                name: "BulletListSettingsModel",
                newName: "BulletListSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NumberedListSettings",
                table: "NumberedListSettings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BulletListSettings",
                table: "BulletListSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListSettings_BulletListSettings_BulletListSettingsId",
                table: "ListSettings",
                column: "BulletListSettingsId",
                principalTable: "BulletListSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListSettings_NumberedListSettings_NumberedListSettingsId",
                table: "ListSettings",
                column: "NumberedListSettingsId",
                principalTable: "NumberedListSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_BulletListSettings_BulletListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ListSettings_NumberedListSettings_NumberedListSettingsId",
                table: "ListSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NumberedListSettings",
                table: "NumberedListSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BulletListSettings",
                table: "BulletListSettings");

            migrationBuilder.RenameTable(
                name: "NumberedListSettings",
                newName: "NumberedListSettingsModel");

            migrationBuilder.RenameTable(
                name: "BulletListSettings",
                newName: "BulletListSettingsModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NumberedListSettingsModel",
                table: "NumberedListSettingsModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BulletListSettingsModel",
                table: "BulletListSettingsModel",
                column: "Id");

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
    }
}
