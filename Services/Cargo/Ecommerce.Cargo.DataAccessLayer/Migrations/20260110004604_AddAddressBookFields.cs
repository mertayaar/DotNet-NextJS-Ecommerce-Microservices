using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Cargo.DataAccessLayer.Migrations
{
    
    public partial class AddAddressBookFields : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CargoCustomers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CargoCustomers",
                type: "nvarchar(max)",
                nullable: true);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CargoCustomers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CargoCustomers");
        }
    }
}
