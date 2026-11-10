using System;
using Hazparo.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToAspNetIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_client_profiles_app_users_user_id",
                schema: "public",
                table: "client_profiles");

            migrationBuilder.DropForeignKey(
                name: "fk_professional_profiles_app_users_user_id",
                schema: "public",
                table: "professional_profiles");

            migrationBuilder.DropForeignKey(
                name: "fk_service_requests_app_users_cancelled_by_user_id",
                schema: "public",
                table: "service_requests");

            migrationBuilder.DropPrimaryKey(
                name: "pk_app_user_roles",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "role",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.RenameColumn(
                name: "last_login_at",
                schema: "public",
                table: "app_users",
                newName: "lockout_end");

            migrationBuilder.RenameColumn(
                name: "avatar_object_key",
                schema: "public",
                table: "app_users",
                newName: "security_stamp");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:account_status", "active,deleted,pending,suspended")
                .Annotation("Npgsql:Enum:quote_status", "accepted,declined,expired,sent,withdrawn")
                .Annotation("Npgsql:Enum:service_request_status", "accepted,assigned,cancelled,completed,draft,en_route,expired,in_progress,open,quoted")
                .Annotation("Npgsql:Enum:verification_status", "not_submitted,pending,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:account_status", "active,deleted,pending,suspended")
                .OldAnnotation("Npgsql:Enum:quote_status", "accepted,declined,expired,sent,withdrawn")
                .OldAnnotation("Npgsql:Enum:service_request_status", "accepted,assigned,cancelled,completed,draft,en_route,expired,in_progress,open,quoted")
                .OldAnnotation("Npgsql:Enum:user_role", "admin,client,professional")
                .OldAnnotation("Npgsql:Enum:verification_status", "not_submitted,pending,rejected,verified")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<AccountStatus>(
                name: "status",
                schema: "public",
                table: "app_users",
                type: "account_status",
                nullable: false,
                oldClrType: typeof(AccountStatus),
                oldType: "account_status",
                oldDefaultValue: AccountStatus.Pending);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                schema: "public",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComputedColumnSql: "upper(email::text)");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "public",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "public",
                table: "app_users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                schema: "public",
                table: "app_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "concurrency_stamp",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "email_confirmed",
                schema: "public",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lockout_enabled",
                schema: "public",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "normalized_user_name",
                schema: "public",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "phone_number_confirmed",
                schema: "public",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                schema: "public",
                table: "app_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                schema: "public",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "role_id",
                schema: "public",
                table: "app_user_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_app_user_roles",
                schema: "public",
                table: "app_user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.CreateTable(
                name: "app_roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "public",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "app_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "app_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "app_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_user_roles_role_id",
                schema: "public",
                table: "app_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "app_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                schema: "public",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                schema: "public",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                schema: "public",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_app_user_roles_app_roles_role_id",
                schema: "public",
                table: "app_user_roles",
                column: "role_id",
                principalSchema: "public",
                principalTable: "app_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_profiles_users_user_id",
                schema: "public",
                table: "client_profiles",
                column: "user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_professional_profiles_users_user_id",
                schema: "public",
                table: "professional_profiles",
                column: "user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_service_requests_users_cancelled_by_user_id",
                schema: "public",
                table: "service_requests",
                column: "cancelled_by_user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_app_user_roles_app_roles_role_id",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_client_profiles_users_user_id",
                schema: "public",
                table: "client_profiles");

            migrationBuilder.DropForeignKey(
                name: "fk_professional_profiles_users_user_id",
                schema: "public",
                table: "professional_profiles");

            migrationBuilder.DropForeignKey(
                name: "fk_service_requests_users_cancelled_by_user_id",
                schema: "public",
                table: "service_requests");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_roles",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_app_user_roles",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.DropIndex(
                name: "ix_app_user_roles_role_id",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.DropColumn(
                name: "access_failed_count",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "concurrency_stamp",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "email_confirmed",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "lockout_enabled",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "normalized_user_name",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "phone_number_confirmed",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "user_name",
                schema: "public",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "role_id",
                schema: "public",
                table: "app_user_roles");

            migrationBuilder.RenameColumn(
                name: "security_stamp",
                schema: "public",
                table: "app_users",
                newName: "avatar_object_key");

            migrationBuilder.RenameColumn(
                name: "lockout_end",
                schema: "public",
                table: "app_users",
                newName: "last_login_at");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:account_status", "active,deleted,pending,suspended")
                .Annotation("Npgsql:Enum:quote_status", "accepted,declined,expired,sent,withdrawn")
                .Annotation("Npgsql:Enum:service_request_status", "accepted,assigned,cancelled,completed,draft,en_route,expired,in_progress,open,quoted")
                .Annotation("Npgsql:Enum:user_role", "admin,client,professional")
                .Annotation("Npgsql:Enum:verification_status", "not_submitted,pending,rejected,verified")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:account_status", "active,deleted,pending,suspended")
                .OldAnnotation("Npgsql:Enum:quote_status", "accepted,declined,expired,sent,withdrawn")
                .OldAnnotation("Npgsql:Enum:service_request_status", "accepted,assigned,cancelled,completed,draft,en_route,expired,in_progress,open,quoted")
                .OldAnnotation("Npgsql:Enum:verification_status", "not_submitted,pending,rejected,verified")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<AccountStatus>(
                name: "status",
                schema: "public",
                table: "app_users",
                type: "account_status",
                nullable: false,
                defaultValue: AccountStatus.Pending,
                oldClrType: typeof(AccountStatus),
                oldType: "account_status");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                schema: "public",
                table: "app_users",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "public",
                table: "app_users",
                type: "citext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                schema: "public",
                table: "app_users",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                schema: "public",
                table: "app_users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "public",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "public",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<int>(
                name: "role",
                schema: "public",
                table: "app_user_roles",
                type: "user_role",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                schema: "public",
                table: "app_user_roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "normalized_email",
                schema: "public",
                table: "app_users",
                type: "text",
                nullable: true,
                computedColumnSql: "upper(email::text)",
                stored: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_app_user_roles",
                schema: "public",
                table: "app_user_roles",
                columns: new[] { "user_id", "role" });

            migrationBuilder.AddForeignKey(
                name: "fk_client_profiles_app_users_user_id",
                schema: "public",
                table: "client_profiles",
                column: "user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_professional_profiles_app_users_user_id",
                schema: "public",
                table: "professional_profiles",
                column: "user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_service_requests_app_users_cancelled_by_user_id",
                schema: "public",
                table: "service_requests",
                column: "cancelled_by_user_id",
                principalSchema: "public",
                principalTable: "app_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
