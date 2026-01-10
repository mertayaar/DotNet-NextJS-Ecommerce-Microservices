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
    public class EfCargoDetailDal : GenericRepository<CargoDetail>, ICargoDetailDal
    {
        public EfCargoDetailDal(CargoContext context) : base(context)
        {
        }
    }
}
