using Ecommerce.Cargo.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Cargo.DataAccessLayer.Abstract
{
    public  interface ICargoCustomerDal : IGenericDal<CargoCustomer>
    {
        CargoCustomer GetByUserCustomerId(string id);
        List<CargoCustomer> GetAllByUserCustomerId(string id);
        void SetDefault(int id, string userId);
    }
}
