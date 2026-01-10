using Ecommerce.Cargo.BusinessLayer.Abstract;
using Ecommerce.Cargo.DtoLayer.Dtos.CargoDetailDtos;
using Ecommerce.Cargo.EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Cargo.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CargoDetailsController : ControllerBase
    {
        private readonly ICargoDetailService _cargoDetailService;

        public CargoDetailsController(ICargoDetailService cargoDetailService)
        {
            _cargoDetailService = cargoDetailService;
        }

        [HttpGet]
        public IActionResult CompanyList()
        {
            var values = _cargoDetailService.TGetAll();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetCargoDetailById(int id)
        {
            var values = _cargoDetailService.TGetById(id);
            return Ok(values);
        }

        [HttpPost]
        public IActionResult CreateCargoDetail(CreateCargoDetailDto createCargoDetailDto)
        {
            CargoDetail cargoDetail = new CargoDetail()
            {
                Barcode = createCargoDetailDto.Barcode,
                CargoCompanyId = createCargoDetailDto.CargoCompanyId,
                ReceiverCustomer = createCargoDetailDto.ReceiverCustomer,
                SenderCustomer = createCargoDetailDto.SenderCustomer    
            };
            _cargoDetailService.TInsert(cargoDetail);
            return Ok();
        }

        [HttpDelete]
        public IActionResult RemoveCargoDetail(int id)
        {
            _cargoDetailService.TRemove(id);
            return Ok();

        }

        [HttpPut]
        public IActionResult UpdateCargoDetail(UpdateCargoDetailDto updateCargoDetailDto)
        {
            CargoDetail cargoDetail = new CargoDetail()
            {
               Barcode= updateCargoDetailDto.Barcode,
               CargoCompanyId = updateCargoDetailDto.CargoCompanyId,
               CargoDetailId = updateCargoDetailDto.CargoDetailId,
               ReceiverCustomer = updateCargoDetailDto.ReceiverCustomer,
               SenderCustomer = updateCargoDetailDto.SenderCustomer 

            };
            _cargoDetailService.TUpdate(cargoDetail);

            return Ok();
        }
    }
}
