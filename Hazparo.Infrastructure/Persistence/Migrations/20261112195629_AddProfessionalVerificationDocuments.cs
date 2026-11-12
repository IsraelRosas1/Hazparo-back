using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalVerificationDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professional_verification_documents",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_professional_verification_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_professional_verification_documents_professional_profiles_p",
                        column: x => x.professional_id,
                        principalSchema: "public",
                        principalTable: "professional_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_professional_verification_documents_professional_id_documen",
                schema: "public",
                table: "professional_verification_documents",
                columns: new[] { "professional_id", "document_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "professional_verification_documents",
                schema: "public");
        }
    }
}
