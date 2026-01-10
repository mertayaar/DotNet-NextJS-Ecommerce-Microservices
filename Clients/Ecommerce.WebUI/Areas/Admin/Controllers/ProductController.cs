using Ecommerce.DtoLayer.CatalogDtos.CategoryDtos;
using Ecommerce.DtoLayer.CatalogDtos.ProductDtos;
using Ecommerce.WebUI.Services.CatalogServices.CategoryServices;
using Ecommerce.WebUI.Services.CatalogServices.ProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using Ecommerce.WebUI.Services.ImageUpload;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]    [Route("Admin/Product")]
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IImageUploadService _imageUploadService;
        

        public ProductController(IProductService productService, ICategoryService categoryService, IImageUploadService imageUploadService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _imageUploadService = imageUploadService;
        }

        void ProductViewBagList()
        {
            ViewBag.v0 = "Product Operation";
            ViewBag.v1 = "Home Page";
            ViewBag.v2 = "Products";
            ViewBag.v3 = "Product List";
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ProductViewBagList();
            var values = await _productService.GetAllProductAsync();
            return View(values);
        }

        [Route("ProductListWithCategory")]
        public async Task<IActionResult> ProductListWithCategory()
        {
            ProductViewBagList();

            var values = await _productService.GetProductsWithCategoryAsync();
            return View(values);
        }



        [Route("CreateProduct")]
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ProductViewBagList();
            var categories = await _categoryService.GetAllCategoryAsync();
            List<SelectListItem> categoryValues = (from x in categories
                                                   select new SelectListItem
                                                   {
                                                       Text = x.CategoryName,
                                                       Value = x.CategoryId
                                                   }).ToList();

            ViewBag.CategoryValues = categoryValues;
            return View();
        }


        [Route("CreateProduct")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto, IFormFile ProductImage)
        {
            Console.WriteLine($"[CreateProduct] Called. ProductImage null? {ProductImage == null}, Count: {ProductImage?.Length}");
            
            if (ProductImage != null && ProductImage.Length > 0)
            {
                Console.WriteLine($"[CreateProduct] Uploading image: {ProductImage.FileName}");
                var imageUrl = await _imageUploadService.UploadAsync(ProductImage, "products");
                Console.WriteLine($"[CreateProduct] Upload result URL: {imageUrl}");
                
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    createProductDto.ProductImageUrl = imageUrl;
                }
            }
            else
            {
                 Console.WriteLine("[CreateProduct] No image provided or empty.");
            }
            
            await _productService.CreateProductAsync(createProductDto);
            return RedirectToAction("ProductListWithCategory", "Product", new { area = "Admin" });
        }

        [Route("DeleteProduct/{id}")]

        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction("ProductListWithCategory", "Product", new { area = "Admin" });
        }

        [Route("UpdateProduct/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(string id)
        {
            ProductViewBagList();
            var categories = await _categoryService.GetAllCategoryAsync();
            List<SelectListItem> categoryValues = (from x in categories
                                                   select new SelectListItem
                                                   {
                                                       Text = x.CategoryName,
                                                       Value = x.CategoryId
                                                   }).ToList();
            ViewBag.CategoryValues = categoryValues;

            var productValues = await _productService.GetByIdProductAsync(id);
            return View(productValues);
        }

        [Route("UpdateProduct/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto, IFormFile? ProductImage)
        {
            if (ProductImage != null && ProductImage.Length > 0)
            {
                var imageUrl = await _imageUploadService.UploadAsync(ProductImage, "products");
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    updateProductDto.ProductImageUrl = imageUrl;
                }
            }
            
            
            await _productService.UpdateProductAsync(updateProductDto);

            return RedirectToAction("ProductListWithCategory", "Product", new { area = "Admin" });
        }
    }
}
