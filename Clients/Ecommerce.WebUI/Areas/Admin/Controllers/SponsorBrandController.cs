using Ecommerce.DtoLayer.CatalogDtos.SponsorBrandDtos;
using Ecommerce.WebUI.Services.CatalogServices.SponsorBrandServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]    [Route("Admin/SponsorBrand")]
    public class SponsorBrandController : Controller
    {
        private readonly ISponsorBrandService _sponsorBrandService;

        public SponsorBrandController(ISponsorBrandService sponsorBrandService)
        {
            _sponsorBrandService = sponsorBrandService;
        }
        void SponsorBrandViewBagList()
        {
            ViewBag.v0 = "SponsorBrand Operation";
            ViewBag.v1 = "Home Page";
            ViewBag.v2 = "Sponsor Brands";
            ViewBag.v3 = "SponsorBrand List";
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            SponsorBrandViewBagList();

            var values = await _sponsorBrandService.GetAllSponsorBrandAsync();
            return View(values);
        }


        [HttpGet]
        [Route("CreateSponsorBrand")]

        public IActionResult CreateSponsorBrand()
        {
            SponsorBrandViewBagList();
            return View();
        }

        [HttpPost]
        [Route("CreateSponsorBrand")]

        public async Task<IActionResult> CreateSponsorBrand(CreateSponsorBrandDto createSponsorBrandDto)
        {
            await _sponsorBrandService.CreateSponsorBrandAsync(createSponsorBrandDto);
            return RedirectToAction("Index", "SponsorBrand", new { area = "Admin" });


        }

        [Route("DeleteSponsorBrand/{id}")]
        public async Task<IActionResult> DeleteSponsorBrand(string id)
        {
            await _sponsorBrandService.DeleteSponsorBrandAsync(id);
            return RedirectToAction("Index", "SponsorBrand", new { area = "Admin" });

        }

        [Route("UpdateSponsorBrand/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateSponsorBrand(string id)
        {
            SponsorBrandViewBagList();
            var values = await _sponsorBrandService.GetByIdSponsorBrandAsync(id);
            return View(values);

        }

        [Route("UpdateSponsorBrand/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateSponsorBrand(UpdateSponsorBrandDto updateSponsorBrandDto)
        {
            await _sponsorBrandService.UpdateSponsorBrandAsync(updateSponsorBrandDto);

            return RedirectToAction("Index", "SponsorBrand", new { area = "Admin" });

        }

    }
}
