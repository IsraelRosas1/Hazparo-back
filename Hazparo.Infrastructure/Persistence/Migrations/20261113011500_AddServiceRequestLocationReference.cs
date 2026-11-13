using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hazparo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRequestLocationReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location_reference",
                schema: "public",
                table: "service_requests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location_reference",
                schema: "public",
                table: "service_requests");
        }
    }
}
