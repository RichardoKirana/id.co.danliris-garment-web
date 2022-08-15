using Microsoft.EntityFrameworkCore.Migrations;

namespace DanLiris.Admin.Web.Migrations
{
    public partial class add_column_QtyPacking_UomSatuanUnit_GarmentSubconDLO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtyPacking",
                table: "GarmentSubconDeliveryLetterOutItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UomSatuanId",
                table: "GarmentSubconDeliveryLetterOutItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UomSatuanUnit",
                table: "GarmentSubconDeliveryLetterOutItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QtyPacking",
                table: "GarmentSubconDeliveryLetterOutItems");

            migrationBuilder.DropColumn(
                name: "UomSatuanId",
                table: "GarmentSubconDeliveryLetterOutItems");

            migrationBuilder.DropColumn(
                name: "UomSatuanUnit",
                table: "GarmentSubconDeliveryLetterOutItems");
        }
    }
}
