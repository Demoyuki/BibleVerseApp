// ============================================================
// File: SearchController.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: MVC Controller handling Bible verse keyword
//              search functionality. Uses IBibleVerseDAO via DI.
// ============================================================

using BibleVerseApp.DAL;
using BibleVerseApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibleVerseApp.Controllers
{
    /// <summary>
    /// Handles the Search page (keyword/phrase searching across Bible verses).
    /// Renders the search form and processes POST submissions.
    /// </summary>
    public class SearchController : Controller
    {
        // DAL dependency for querying Bible verses
        private readonly IBibleVerseDAO _verseDAO;

        /// <summary>
        /// Constructor uses dependency injection to receive the IBibleVerseDAO implementation.
        /// </summary>
        /// <param name="verseDAO">Injected IBibleVerseDAO implementation.</param>
        public SearchController(IBibleVerseDAO verseDAO)
        {
            // Assign the injected DAO to the private field
            _verseDAO = verseDAO;
        }

        /// <summary>
        /// GET: /Search
        /// Displays the blank search form with an empty SearchViewModel.
        /// </summary>
        /// <returns>Search/Index view with an empty view model.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Return empty form; no search has been run yet
            return View(new SearchViewModel());
        }

        /// <summary>
        /// GET: /Search/Results
        /// Accepts the search form data, runs the query, and returns results.
        /// </summary>
        /// <param name="vm">SearchViewModel populated from the form GET.</param>
        /// <returns>Search/Results view with populated Results list.</returns>
        [HttpGet]
        public IActionResult Results(SearchViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.SearchTerm))
            {
                vm.Results = _verseDAO.SearchVerses(
                    vm.SearchTerm,
                    vm.IncludeOldTestament,
                    vm.IncludeNewTestament
                );
            }

            return View("Index", vm);
        }
    }
}