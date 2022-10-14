using Microsoft.EntityFrameworkCore.Migrations;

namespace DanLiris.Admin.Web.Migrations
{
    public partial class addcolumnNWGWServiceShrinkagePanel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GrossWeight",
                table: "GarmentServiceSubconShrinkagePanels",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "NettWeight",
                table: "GarmentServiceSubconShrinkagePanels",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "GarmentServiceSubconShrinkagePanels");

            migrationBuilder.DropColumn(
                name: "NettWeight",
                table: "GarmentServiceSubconShrinkagePanels");
        }
    }
}
