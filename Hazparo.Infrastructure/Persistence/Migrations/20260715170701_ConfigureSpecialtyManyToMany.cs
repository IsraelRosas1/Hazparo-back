using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureSpecialtyManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Professionals_ProfessionalId",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_ProfessionalId",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ProfessionalId",
                table: "Specialties");

            migrationBuilder.CreateTable(
                name: "ProfessionalSpecialties",
                columns: table => new
                {
                    ProfessionalsId = table.Column<int>(type: "int", nullable: false),
                    SpecialtiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalSpecialties", x => new { x.ProfessionalsId, x.SpecialtiesId });
                    table.ForeignKey(
                        name: "FK_ProfessionalSpecialties_Professionals_ProfessionalsId",
                        column: x => x.ProfessionalsId,
                        principalTable: "Professionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionalSpecialties_Specialties_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalSpecialties_SpecialtiesId",
                table: "ProfessionalSpecialties",
                column: "SpecialtiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionalSpecialties");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionalId",
                table: "Specialties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_ProfessionalId",
                table: "Specialties",
                column: "ProfessionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_Professionals_ProfessionalId",
                table: "Specialties",
                column: "ProfessionalId",
                principalTable: "Professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
