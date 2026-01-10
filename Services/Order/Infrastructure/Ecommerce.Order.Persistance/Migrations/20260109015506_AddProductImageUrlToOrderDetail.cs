using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Order.Persistance.Migrations
{
    
    public partial class AddProductImageUrlToOrderDetail : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductUrl",
                table: "OrderDetails",
                newName: "ProductImageUrl");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductImageUrl",
                table: "OrderDetails",
                newName: "ProductUrl");
        }
    }
}
