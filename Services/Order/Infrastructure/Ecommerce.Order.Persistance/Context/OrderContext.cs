using Ecommerce.Order.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Order.Persistance.Context
{
    public class OrderContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public OrderContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
            
            _connectionString = _configuration["OrderConnectionStrings:DefaultConnection"] 
                ?? _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Order database connection string not configured. Set OrderConnectionStrings__DefaultConnection in environment.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Ordering> Orderings { get; set; }

    }
}
