using Microsoft.AspNetCore.Authorization;
﻿using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]    public class AdminLayout : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
