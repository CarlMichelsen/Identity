using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "content",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    authentication_provider_id = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    authentication_provider = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    raw_avatar_url = table.Column<string>(type: "text", nullable: false),
                    image_id = table.Column<Guid>(type: "uuid", nullable: true),
                    image_processing_attempts = table.Column<int>(type: "integer", nullable: false),
                    roles = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    small_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medium_id = table.Column<Guid>(type: "uuid", nullable: false),
                    large_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image", x => x.id);
                    table.ForeignKey(
                        name: "fk_image_content_large_id",
                        column: x => x.large_id,
                        principalSchema: "identity",
                        principalTable: "content",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_content_medium_id",
                        column: x => x.medium_id,
                        principalSchema: "identity",
                        principalTable: "content",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_content_small_id",
                        column: x => x.small_id,
                        principalSchema: "identity",
                        principalTable: "content",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "o_auth_process",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    authentication_provider = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    login_redirect_uri = table.Column<string>(type: "text", nullable: false),
                    success_redirect_uri = table.Column<string>(type: "text", nullable: false),
                    error_redirect_uri = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    login_id = table.Column<Guid>(type: "uuid", nullable: true),
                    error = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: true),
                    full_user_json = table.Column<string>(type: "jsonb", maxLength: 2147483647, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_o_auth_process", x => x.id);
                    table.ForeignKey(
                        name: "fk_o_auth_process_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "login",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    o_auth_process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_login = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login", x => x.id);
                    table.ForeignKey(
                        name: "fk_login_o_auth_process_o_auth_process_id",
                        column: x => x.o_auth_process_id,
                        principalSchema: "identity",
                        principalTable: "o_auth_process",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_login_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hashed_refresh_token = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    login_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valid_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    remote_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    remote_port = table.Column<int>(type: "integer", nullable: false),
                    user_agent = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_login_login_id",
                        column: x => x.login_id,
                        principalSchema: "identity",
                        principalTable: "login",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_refresh_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "access",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    refresh_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valid_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    remote_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    remote_port = table.Column<int>(type: "integer", nullable: false),
                    user_agent = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access", x => x.id);
                    table.ForeignKey(
                        name: "fk_access_login_login_id",
                        column: x => x.login_id,
                        principalSchema: "identity",
                        principalTable: "login",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_access_refresh_refresh_id",
                        column: x => x.refresh_id,
                        principalSchema: "identity",
                        principalTable: "refresh",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_access_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_login_id",
                schema: "identity",
                table: "access",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_refresh_id",
                schema: "identity",
                table: "access",
                column: "refresh_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_user_id",
                schema: "identity",
                table: "access",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_image_large_id",
                schema: "identity",
                table: "image",
                column: "large_id");

            migrationBuilder.CreateIndex(
                name: "ix_image_medium_id",
                schema: "identity",
                table: "image",
                column: "medium_id");

            migrationBuilder.CreateIndex(
                name: "ix_image_small_id",
                schema: "identity",
                table: "image",
                column: "small_id");

            migrationBuilder.CreateIndex(
                name: "ix_image_user_id",
                schema: "identity",
                table: "image",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_login_o_auth_process_id",
                schema: "identity",
                table: "login",
                column: "o_auth_process_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_login_user_id",
                schema: "identity",
                table: "login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_o_auth_process_state",
                schema: "identity",
                table: "o_auth_process",
                column: "state",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_o_auth_process_user_id",
                schema: "identity",
                table: "o_auth_process",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_login_id",
                schema: "identity",
                table: "refresh",
                column: "login_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_user_id",
                schema: "identity",
                table: "refresh",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "image",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "refresh",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "content",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "login",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "o_auth_process",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user",
                schema: "identity");
        }
    }
}
