using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Review.Migrations
{
    
    public partial class UpdateReviewDb : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "UserReviews",
                newName: "Status");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserReviews",
                newName: "status");
        }
    }
}
