using Ecommerce.Cargo.DataAccessLayer.Abstract;
using Ecommerce.Cargo.DataAccessLayer.Concrete;
using Ecommerce.Cargo.DataAccessLayer.Repositores;
using Ecommerce.Cargo.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Cargo.DataAccessLayer.EntityFramework
{
    public class EfCargoCustomerDal : GenericRepository<CargoCustomer>, ICargoCustomerDal
    {
        private readonly CargoContext _context;

        public EfCargoCustomerDal(CargoContext context) : base(context)
        {
            _context = context;
        }

        public CargoCustomer GetByUserCustomerId(string id)
        {
            var values = _context.CargoCustomers.Where(x => x.UserCustomerId == id).FirstOrDefault();
            return values;
        }

        public List<CargoCustomer> GetAllByUserCustomerId(string id)
        {
            return _context.CargoCustomers
                .Where(x => x.UserCustomerId == id)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CargoCustomerId)
                .ToList();
        }

        public void SetDefault(int id, string userId)
        {
            
            var userAddresses = _context.CargoCustomers.Where(x => x.UserCustomerId == userId).ToList();
            foreach (var addr in userAddresses)
            {
                addr.IsDefault = false;
            }
            
            
            var targetAddress = userAddresses.FirstOrDefault(x => x.CargoCustomerId == id);
            if (targetAddress != null)
            {
                targetAddress.IsDefault = true;
            }
            
            _context.SaveChanges();
        }
    }
}
