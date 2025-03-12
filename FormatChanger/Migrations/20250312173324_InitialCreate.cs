using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormatChanger.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    TelegramUserName = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HasPageNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    HasImageCaptions = table.Column<bool>(type: "boolean", nullable: false),
                    HasTableCaptions = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationSystems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeaderWeight = table.Column<int>(type: "integer", nullable: false),
                    TextWeight = table.Column<int>(type: "integer", nullable: false),
                    ImageWeight = table.Column<int>(type: "integer", nullable: false),
                    TableWeight = table.Column<int>(type: "integer", nullable: false),
                    ListWeight = table.Column<int>(type: "integer", nullable: false),
                    FreeCoefficient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LineSpacing = table.Column<float>(type: "real", nullable: false),
                    BeforeSpacing = table.Column<float>(type: "real", nullable: false),
                    AfterSpacing = table.Column<float>(type: "real", nullable: false),
                    Justification = table.Column<string>(type: "text", nullable: false),
                    Left = table.Column<float>(type: "real", nullable: false),
                    Right = table.Column<float>(type: "real", nullable: false),
                    FirstLine = table.Column<float>(type: "real", nullable: false),
                    KeepWithNext = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Font = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    IsBold = table.Column<bool>(type: "boolean", nullable: false),
                    IsItalic = table.Column<bool>(type: "boolean", nullable: false),
                    IsUnderscore = table.Column<bool>(type: "boolean", nullable: false),
                    FontSize = table.Column<float>(type: "real", nullable: false),
                    LineSpacing = table.Column<float>(type: "real", nullable: false),
                    BeforeSpacing = table.Column<float>(type: "real", nullable: false),
                    AfterSpacing = table.Column<float>(type: "real", nullable: false),
                    Justification = table.Column<string>(type: "text", nullable: false),
                    Left = table.Column<float>(type: "real", nullable: false),
                    Right = table.Column<float>(type: "real", nullable: false),
                    FirstLine = table.Column<float>(type: "real", nullable: false),
                    KeepWithNext = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    IsOriginal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaptionSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    TextTemplate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaptionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaptionSettings_TextSettings_TextSettingsId",
                        column: x => x.TextSettingsId,
                        principalTable: "TextSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    VerticalAlignment = table.Column<string>(type: "text", nullable: false),
                    LeftPadding = table.Column<int>(type: "integer", nullable: false),
                    RightPadding = table.Column<int>(type: "integer", nullable: false),
                    BottomPadding = table.Column<int>(type: "integer", nullable: false),
                    TopPadding = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellSettings_TextSettings_TextSettingsId",
                        column: x => x.TextSettingsId,
                        principalTable: "TextSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeadingSettingsModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    StartOnNewPage = table.Column<bool>(type: "boolean", nullable: false),
                    HeadingLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadingSettingsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeadingSettingsModel_TextSettings_TextSettingsId",
                        column: x => x.TextSettingsId,
                        principalTable: "TextSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    IsNumeric = table.Column<bool>(type: "boolean", nullable: false),
                    MarkerType = table.Column<string>(type: "text", nullable: true),
                    ListLevel = table.Column<int>(type: "integer", nullable: false),
                    EndType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListSettings_TextSettings_TextSettingsId",
                        column: x => x.TextSettingsId,
                        principalTable: "TextSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeaderSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CellSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    HasRepetitions = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeaderSettings_CellSettings_CellSettingsId",
                        column: x => x.CellSettingsId,
                        principalTable: "CellSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BeforeSpacing = table.Column<float>(type: "real", nullable: false),
                    AfterSpacing = table.Column<float>(type: "real", nullable: false),
                    CaptionSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    CellSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    HeaderSettingsModelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableSettings_CaptionSettings_CaptionSettingsId",
                        column: x => x.CaptionSettingsId,
                        principalTable: "CaptionSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableSettings_CellSettings_CellSettingsId",
                        column: x => x.CellSettingsId,
                        principalTable: "CellSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableSettings_HeaderSettings_HeaderSettingsModelId",
                        column: x => x.HeaderSettingsModelId,
                        principalTable: "HeaderSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormattingTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    HeadingSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    TableSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    ListSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    ImageSettingsId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentSettingsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormattingTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_DocumentSettings_DocumentSettingsId",
                        column: x => x.DocumentSettingsId,
                        principalTable: "DocumentSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_HeadingSettingsModel_HeadingSettingsId",
                        column: x => x.HeadingSettingsId,
                        principalTable: "HeadingSettingsModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_ImageSettings_ImageSettingsId",
                        column: x => x.ImageSettingsId,
                        principalTable: "ImageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_ListSettings_ListSettingsId",
                        column: x => x.ListSettingsId,
                        principalTable: "ListSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_TableSettings_TableSettingsId",
                        column: x => x.TableSettingsId,
                        principalTable: "TableSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormattingTemplates_TextSettings_TextSettingsId",
                        column: x => x.TextSettingsId,
                        principalTable: "TextSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrectionResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormattingTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    CorrectedDocumentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrectionResults_Documents_CorrectedDocumentId",
                        column: x => x.CorrectedDocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorrectionResults_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorrectionResults_FormattingTemplates_FormattingTemplateId",
                        column: x => x.FormattingTemplateId,
                        principalTable: "FormattingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    FormattingTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    EvaluationSystemModelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_EvaluationSystems_EvaluationSystemModelId",
                        column: x => x.EvaluationSystemModelId,
                        principalTable: "EvaluationSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationResults_FormattingTemplates_FormattingTemplateId",
                        column: x => x.FormattingTemplateId,
                        principalTable: "FormattingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaptionSettings_TextSettingsId",
                table: "CaptionSettings",
                column: "TextSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CellSettings_TextSettingsId",
                table: "CellSettings",
                column: "TextSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectionResults_CorrectedDocumentId",
                table: "CorrectionResults",
                column: "CorrectedDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectionResults_DocumentId",
                table: "CorrectionResults",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectionResults_FormattingTemplateId",
                table: "CorrectionResults",
                column: "FormattingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId",
                table: "Documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_DocumentId",
                table: "EvaluationResults",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_EvaluationSystemModelId",
                table: "EvaluationResults",
                column: "EvaluationSystemModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResults_FormattingTemplateId",
                table: "EvaluationResults",
                column: "FormattingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_DocumentSettingsId",
                table: "FormattingTemplates",
                column: "DocumentSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_HeadingSettingsId",
                table: "FormattingTemplates",
                column: "HeadingSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_ImageSettingsId",
                table: "FormattingTemplates",
                column: "ImageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_ListSettingsId",
                table: "FormattingTemplates",
                column: "ListSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_TableSettingsId",
                table: "FormattingTemplates",
                column: "TableSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FormattingTemplates_TextSettingsId",
                table: "FormattingTemplates",
                column: "TextSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderSettings_CellSettingsId",
                table: "HeaderSettings",
                column: "CellSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadingSettingsModel_TextSettingsId",
                table: "HeadingSettingsModel",
                column: "TextSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ListSettings_TextSettingsId",
                table: "ListSettings",
                column: "TextSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSettings_CaptionSettingsId",
                table: "TableSettings",
                column: "CaptionSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSettings_CellSettingsId",
                table: "TableSettings",
                column: "CellSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSettings_HeaderSettingsModelId",
                table: "TableSettings",
                column: "HeaderSettingsModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CorrectionResults");

            migrationBuilder.DropTable(
                name: "EvaluationResults");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "EvaluationSystems");

            migrationBuilder.DropTable(
                name: "FormattingTemplates");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DocumentSettings");

            migrationBuilder.DropTable(
                name: "HeadingSettingsModel");

            migrationBuilder.DropTable(
                name: "ImageSettings");

            migrationBuilder.DropTable(
                name: "ListSettings");

            migrationBuilder.DropTable(
                name: "TableSettings");

            migrationBuilder.DropTable(
                name: "CaptionSettings");

            migrationBuilder.DropTable(
                name: "HeaderSettings");

            migrationBuilder.DropTable(
                name: "CellSettings");

            migrationBuilder.DropTable(
                name: "TextSettings");
        }
    }
}
