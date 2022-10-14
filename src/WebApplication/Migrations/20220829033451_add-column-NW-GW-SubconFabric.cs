using Microsoft.EntityFrameworkCore.Migrations;

namespace DanLiris.Admin.Web.Migrations
{
    public partial class addcolumnNWGWSubconFabric : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GrossWeight",
                table: "GarmentServiceSubconFabricWashes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NettWeight",
                table: "GarmentServiceSubconFabricWashes",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "GarmentServiceSubconFabricWashes");

            migrationBuilder.DropColumn(
                name: "NettWeight",
                table: "GarmentServiceSubconFabricWashes");
        }
    }
}
