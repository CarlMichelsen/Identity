using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class IdentityInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider_id = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    authentication_provider = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                    email = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_record",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    o_auth_json = table.Column<string>(type: "jsonb", maxLength: 1048576, nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invalidated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_record", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_record_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "old_information_record_entity",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    information = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                    replaced_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_old_information_record_entity", x => x.id);
                    table.ForeignKey(
                        name: "fk_old_information_record_entity_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_record",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_record_id = table.Column<long>(type: "bigint", nullable: false),
                    expires_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    invalidated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_record", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_record_login_record_login_record_id",
                        column: x => x.login_record_id,
                        principalSchema: "identity",
                        principalTable: "login_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "access_record",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    refresh_record_id = table.Column<long>(type: "bigint", nullable: false),
                    expires_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(2056)", maxLength: 2056, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_record", x => x.id);
                    table.ForeignKey(
                        name: "fk_access_record_refresh_record_refresh_record_id",
                        column: x => x.refresh_record_id,
                        principalSchema: "identity",
                        principalTable: "refresh_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_record_refresh_record_id",
                schema: "identity",
                table: "access_record",
                column: "refresh_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_login_record_user_id",
                schema: "identity",
                table: "login_record",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_old_information_record_entity_user_id",
                schema: "identity",
                table: "old_information_record_entity",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_record_login_record_id",
                schema: "identity",
                table: "refresh_record",
                column: "login_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_provider_id_authentication_provider",
                schema: "identity",
                table: "user",
                columns: new[] { "provider_id", "authentication_provider" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_record",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "old_information_record_entity",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "refresh_record",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "login_record",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user",
                schema: "identity");
        }
    }
}
