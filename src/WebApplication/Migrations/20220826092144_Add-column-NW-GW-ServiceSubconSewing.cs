using Microsoft.EntityFrameworkCore.Migrations;

namespace DanLiris.Admin.Web.Migrations
{
    public partial class AddcolumnNWGWServiceSubconSewing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GrossWeight",
                table: "GarmentServiceSubconSewings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NettWeight",
                table: "GarmentServiceSubconSewings",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "GarmentServiceSubconSewings");

            migrationBuilder.DropColumn(
                name: "NettWeight",
                table: "GarmentServiceSubconSewings");
        }
    }
}
