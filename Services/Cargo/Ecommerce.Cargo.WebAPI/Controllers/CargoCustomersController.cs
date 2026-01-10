 using Ecommerce.Cargo.BusinessLayer.Abstract;
using Ecommerce.Cargo.DtoLayer.Dtos.CargoCustomerDtos;
using Ecommerce.Cargo.EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Cargo.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CargoCustomersController : ControllerBase
    {
        private readonly ICargoCustomerService _cargoCustomerService;

        public CargoCustomersController(ICargoCustomerService cargoCustomerService)
        {
            _cargoCustomerService = cargoCustomerService;
        }

        [HttpGet]
        public IActionResult CompanyList()
        {
            var values = _cargoCustomerService.TGetAll();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetCargoCustomerById(int id)
        {
            var values = _cargoCustomerService.TGetById(id);
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateCargoCustomer(CreateCargoCustomerDto createCargoCustomerDto)
        {
            
            if (createCargoCustomerDto.IsDefault && !string.IsNullOrEmpty(createCargoCustomerDto.UserCustomerId))
            {
                var existingAddresses = _cargoCustomerService.TGetAllByUserCustomerId(createCargoCustomerDto.UserCustomerId);
                foreach (var addr in existingAddresses.Where(a => a.IsDefault))
                {
                    addr.IsDefault = false;
                    _cargoCustomerService.TUpdate(addr);
                }
            }

            
            var addressCount = string.IsNullOrEmpty(createCargoCustomerDto.UserCustomerId) 
                ? 0 
                : _cargoCustomerService.TGetAllByUserCustomerId(createCargoCustomerDto.UserCustomerId).Count;

            CargoCustomer CargoCustomer = new CargoCustomer()
            {
                Title = createCargoCustomerDto.Title,
                Address = createCargoCustomerDto.Address,
                City = createCargoCustomerDto.City,
                District = createCargoCustomerDto.District,
                Email = createCargoCustomerDto.Email,
                Phone = createCargoCustomerDto.Phone,
                Name = createCargoCustomerDto.Name,
                Surname = createCargoCustomerDto.Surname,
                UserCustomerId = createCargoCustomerDto.UserCustomerId,
                IsDefault = createCargoCustomerDto.IsDefault || addressCount == 0
            };
            _cargoCustomerService.TInsert(CargoCustomer);
            return Ok(new { success = true, id = CargoCustomer.CargoCustomerId });
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveCargoCustomer(int id)
        {
            var address = _cargoCustomerService.TGetById(id);
            var wasDefault = address?.IsDefault ?? false;
            var userId = address?.UserCustomerId;

            _cargoCustomerService.TRemove(id);

            
            if (wasDefault && !string.IsNullOrEmpty(userId))
            {
                var remaining = _cargoCustomerService.TGetAllByUserCustomerId(userId);
                if (remaining.Any())
                {
                    remaining.First().IsDefault = true;
                    _cargoCustomerService.TUpdate(remaining.First());
                }
            }

            return Ok(new { success = true });
        }

        [HttpPut]
        public IActionResult UpdateCargoCustomer(UpdateCargoCustomerDto updateCargoCustomerDto)
        {
            var cargoCustomer = _cargoCustomerService.TGetById(updateCargoCustomerDto.CargoCustomerId);
            if (cargoCustomer == null)
            {
                return NotFound("Address not found");
            }

            
            if (updateCargoCustomerDto.IsDefault && !string.IsNullOrEmpty(updateCargoCustomerDto.UserCustomerId))
            {
                var existingAddresses = _cargoCustomerService.TGetAllByUserCustomerId(updateCargoCustomerDto.UserCustomerId);
                foreach (var addr in existingAddresses.Where(a => a.IsDefault && a.CargoCustomerId != updateCargoCustomerDto.CargoCustomerId))
                {
                    addr.IsDefault = false;
                    _cargoCustomerService.TUpdate(addr);
                }
            }

            
            cargoCustomer.Title = updateCargoCustomerDto.Title;
            cargoCustomer.Name = updateCargoCustomerDto.Name;
            cargoCustomer.Surname = updateCargoCustomerDto.Surname;
            cargoCustomer.Email = updateCargoCustomerDto.Email;
            cargoCustomer.Phone = updateCargoCustomerDto.Phone;
            cargoCustomer.City = updateCargoCustomerDto.City;
            cargoCustomer.District = updateCargoCustomerDto.District;
            cargoCustomer.Address = updateCargoCustomerDto.Address;
            cargoCustomer.UserCustomerId = updateCargoCustomerDto.UserCustomerId;
            cargoCustomer.IsDefault = updateCargoCustomerDto.IsDefault;

            _cargoCustomerService.TUpdate(cargoCustomer);

            return Ok(new { success = true });
        }

        [HttpGet("GetCargoCustomerByUserCustomerId/{id}")]
        public IActionResult GetCargoCustomerByUserCustomerId(string id)
        {
            var values = _cargoCustomerService.TGetByUserCustomerId(id);
            return Ok(values);
        }

        
        
        
        [HttpGet("user/{userId}/addresses")]
        public IActionResult GetAllAddressesByUser(string userId)
        {
            var values = _cargoCustomerService.TGetAllByUserCustomerId(userId);
            return Ok(values);
        }

        
        
        
        [HttpPost("{id}/set-default")]
        public IActionResult SetDefault(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required");
            }

            _cargoCustomerService.TSetDefault(id, userId);
            return Ok(new { success = true });
        }
    }
}
