using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalLocationAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "city",
                schema: "public",
                table: "professional_locations",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_line1",
                schema: "public",
                table: "professional_locations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "address_line2",
                schema: "public",
                table: "professional_locations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_code",
                schema: "public",
                table: "professional_locations",
                type: "char(2)",
                nullable: false,
                defaultValue: "MX");

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                schema: "public",
                table: "professional_locations",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                schema: "public",
                table: "professional_locations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                schema: "public",
                table: "professional_locations",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address_line1",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.DropColumn(
                name: "address_line2",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.DropColumn(
                name: "country_code",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.DropColumn(
                name: "postal_code",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.DropColumn(
                name: "state",
                schema: "public",
                table: "professional_locations");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                schema: "public",
                table: "professional_locations",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);
        }
    }
}
