using Ecommerce.Cargo.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Cargo.DataAccessLayer.Concrete
{
    public class CargoContext : DbContext 
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CargoContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
            
            _connectionString = _configuration["CargoConnectionStrings:DefaultConnection"] 
                ?? _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cargo database connection string not configured. Set CargoConnectionStrings__DefaultConnection in environment.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
         
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<CargoCompany> CargoCompanies { get; set; }
        public DbSet<CargoCustomer> CargoCustomers { get; set; }
        public DbSet<CargoDetail> CargoDetails { get; set; }
        public DbSet<CargoOperation> CargoOperations { get; set; }
         
    }
}
