using Microsoft.EntityFrameworkCore.Migrations;

namespace ReuseventoryApi.Migrations
{
    public partial class ListingPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "price",
                table: "Listings",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "Listings");
        }
    }
}
