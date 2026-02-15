using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GEntretien.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Equipments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Equipments",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Equipments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Equipments");
        }
    }
}
