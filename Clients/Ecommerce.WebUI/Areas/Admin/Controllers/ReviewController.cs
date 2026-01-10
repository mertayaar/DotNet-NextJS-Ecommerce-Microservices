using Ecommerce.DtoLayer.ReviewDtos;
using Ecommerce.WebUI.Services.ReviewServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]    [Route("Admin/Review")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.v0 = "Review Operation";
            ViewBag.v1 = "Home Page";
            ViewBag.v2 = "Reviews";
            ViewBag.v3 = "Review List";

            var reviewList = await _reviewService.GetAllReviewAsync();
            return View(reviewList);

        }



        [Route("DeleteReview/{id}")]

        public async Task<IActionResult> DeleteReview(string id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return RedirectToAction("Index", "Review", new { area = "Admin" });
        }

        [Route("UpdateReview/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateReview(string id)
        {
            ViewBag.v0 = "Home Page";
            ViewBag.v1 = "Reviews";
            ViewBag.v2 = "Update Review";
            ViewBag.v3 = "Review Operations";

            var review = await _reviewService.GetByIdReviewAsync(id);
            return View(review);
        }

        [Route("UpdateReview/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateReview(UpdateReviewDto updateReviewDto)
        {
            updateReviewDto.Status = true;
            await _reviewService.UpdateReviewAsync(updateReviewDto);
            return RedirectToAction("Index", "Review", new { area = "Admin" });
        }

    }
}
