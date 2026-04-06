
// ============================================================
// File: HomeController.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Default landing page controller.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace BibleVerseApp.Controllers
{
    /// <summary>
    /// Handles the application home/landing page.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: /
        /// Returns the home page view with navigation to Search and Reference.
        /// </summary>
        /// <returns>Home/Index view.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
