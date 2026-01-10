using Ecommerce.Cargo.BusinessLayer.Abstract;
using Ecommerce.Cargo.DataAccessLayer.Abstract;
using Ecommerce.Cargo.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Cargo.BusinessLayer.Concrete
{
    public class CargoCustomerManager : ICargoCustomerService
    {
        private readonly ICargoCustomerDal _cargoCustomerDal;

        public CargoCustomerManager(ICargoCustomerDal cargoCustomerDal)
        {
            _cargoCustomerDal = cargoCustomerDal;
        }

        public List<CargoCustomer> TGetAll()
        {
            return _cargoCustomerDal.GetAll();
        }

        public CargoCustomer TGetById(int id)
        {
            return _cargoCustomerDal.GetById(id);
        }

        public CargoCustomer TGetByUserCustomerId(string id)
        {
            return _cargoCustomerDal.GetByUserCustomerId(id);
        }

        public List<CargoCustomer> TGetAllByUserCustomerId(string id)
        {
            return _cargoCustomerDal.GetAllByUserCustomerId(id);
        }

        public void TSetDefault(int id, string userId)
        {
            _cargoCustomerDal.SetDefault(id, userId);
        }

        public void TInsert(CargoCustomer entity)
        {
            _cargoCustomerDal.Insert(entity);
        }

        public void TRemove(int id)
        {
            _cargoCustomerDal.Remove(id);
        }

        public void TUpdate(CargoCustomer entity)
        {
            _cargoCustomerDal.Update(entity);
        }
    }
}
