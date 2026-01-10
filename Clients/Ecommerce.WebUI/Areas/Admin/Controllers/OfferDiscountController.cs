using Ecommerce.DtoLayer.CatalogDtos.OfferDiscountDtos;
using Ecommerce.WebUI.Services.CatalogServices.OfferDiscountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
 
        [Area("Admin")]
    [Authorize(Roles = "Admin")]        [Route("Admin/OfferDiscount")]
        public class OfferDiscountController : Controller
        {
        private readonly IOfferDiscountService _offerDiscountService;

        public OfferDiscountController(IOfferDiscountService offerDiscountService)
        {
            _offerDiscountService = offerDiscountService;
        }

        void OfferDiscountViewBagList()
            {
                ViewBag.v0 = "OfferDiscount Operation";
                ViewBag.v1 = "Home Page";
                ViewBag.v2 = "OfferDiscounts";
                ViewBag.v3 = "OfferDiscount List";
        }

        [Route("Index")]
            public async Task<IActionResult> Index()
            {
            OfferDiscountViewBagList();

            var values = await _offerDiscountService.GetAllOfferDiscountAsync();
            return View(values);
        }


            [HttpGet]
            [Route("CreateOfferDiscount")]

            public IActionResult CreateOfferDiscount()
            {
            OfferDiscountViewBagList();
                return View();
            }

            [HttpPost]
            [Route("CreateOfferDiscount")]

            public async Task<IActionResult> CreateOfferDiscount(CreateOfferDiscountDto createOfferDiscountDto)
            {
            await _offerDiscountService.CreateOfferDiscountAsync(createOfferDiscountDto);
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }

            [Route("DeleteOfferDiscount/{id}")]

            public async Task<IActionResult> DeleteOfferDiscount(string id)
            {
            await _offerDiscountService.DeleteOfferDiscountAsync(id);
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }

            [Route("UpdateOfferDiscount/{id}")]
            [HttpGet]
            public async Task<IActionResult> UpdateOfferDiscount(string id)
            {
            OfferDiscountViewBagList();
            var values = await _offerDiscountService.GetByIdOfferDiscountAsync(id);
            return View(values);
        }

            [Route("UpdateOfferDiscount/{id}")]
            [HttpPost]
            public async Task<IActionResult> UpdateOfferDiscount(UpdateOfferDiscountDto updateOfferDiscountDto)
            {

            await _offerDiscountService.UpdateOfferDiscountAsync(updateOfferDiscountDto);

            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }

        }
    }
