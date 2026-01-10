using Ecommerce.Review.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Review.Context
{
    public class ReviewContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReviewContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
            
            _connectionString = _configuration["ReviewConnectionStrings:DefaultConnection"] 
                ?? _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Review database connection string not configured. Set ReviewConnectionStrings__DefaultConnection in environment.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<UserReview> UserReviews { get; set; }

        }
}
