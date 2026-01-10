using Ecommerce.Cargo.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Cargo.BusinessLayer.Abstract
{
    public interface ICargoCustomerService : IGenericService<CargoCustomer>
    {
        CargoCustomer TGetByUserCustomerId(string id);
        List<CargoCustomer> TGetAllByUserCustomerId(string id);
        void TSetDefault(int id, string userId);
    }
}
