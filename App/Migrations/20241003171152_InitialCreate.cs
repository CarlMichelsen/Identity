using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    AuthenticationProvider = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                    Email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginRecord",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ip = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginRecord_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshRecord",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoginRecordId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ip = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshRecord_LoginRecord_LoginRecordId",
                        column: x => x.LoginRecordId,
                        principalSchema: "identity",
                        principalTable: "LoginRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginRecord_UserId",
                schema: "identity",
                table: "LoginRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshRecord_LoginRecordId",
                schema: "identity",
                table: "RefreshRecord",
                column: "LoginRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_User_ProviderId_AuthenticationProvider",
                schema: "identity",
                table: "User",
                columns: new[] { "ProviderId", "AuthenticationProvider" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshRecord",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "LoginRecord",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "User",
                schema: "identity");
        }
    }
}