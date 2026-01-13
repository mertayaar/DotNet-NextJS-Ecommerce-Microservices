using Ecommerce.DtoLayer.CatalogDtos.CategoryDtos;
using Ecommerce.WebUI.Services.CatalogServices.CategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/Category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        void CategoryViewBagList()
        {
            ViewBag.v0 = "Category Operation";
            ViewBag.v1 = "Home Page";
            ViewBag.v2 = "Categories";
            ViewBag.v3 = "Category List";
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            CategoryViewBagList();

            var values = await _categoryService.GetAllCategoryAsync();
            return View(values);
        }

        [HttpGet]
        [Route("CreateCategory")]

        public IActionResult CreateCategory()
        {
            CategoryViewBagList();
            return View();
        }

        [HttpPost]
        [Route("CreateCategory")]

        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            await _categoryService.CreateCategoryAsync(createCategoryDto);
            return RedirectToAction("Index", "Category", new { area = "Admin" });

        }

        [Route("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction("Index", "Category", new { area = "Admin" });

        }

        [Route("UpdateCategory/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(string id)
        {
            CategoryViewBagList();
            var values = await _categoryService.GetByIdCategoryAsync(id);
            return View(values);

        }

        [Route("UpdateCategory/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            await _categoryService.UpdateCategoryAsync(updateCategoryDto);

            return RedirectToAction("Index", "Category", new { area = "Admin" });

        }

    }
}
