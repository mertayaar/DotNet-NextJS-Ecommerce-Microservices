using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Review.Migrations
{
    
    public partial class mig2 : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "UserReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "UserReviews");
        }
    }
}
