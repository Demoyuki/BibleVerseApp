// ============================================================
// File: ReferenceController.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: MVC Controller for the Reference lookup feature.
//              Provides Book and Chapter drop-down selection,
//              then returns all verses for the chosen chapter.
// ============================================================

using BibleVerseApp.DAL;
using BibleVerseApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibleVerseApp.Controllers
{
    /// <summary>
    /// Handles Book/Chapter reference navigation.
    /// Allows users to browse any chapter of the Bible by dropdown.
    /// </summary>
    public class ReferenceController : Controller
    {
        // DAO for book metadata (names, chapter counts)
        private readonly IBibleBookDAO _bookDAO;

        // DAO for retrieving verses by chapter
        private readonly IBibleVerseDAO _verseDAO;

        /// <summary>
        /// Injects both required DAO dependencies.
        /// </summary>
        /// <param name="bookDAO">Injected IBibleBookDAO for book/chapter data.</param>
        /// <param name="verseDAO">Injected IBibleVerseDAO for chapter verse retrieval.</param>
        public ReferenceController(IBibleBookDAO bookDAO, IBibleVerseDAO verseDAO)
        {
            _bookDAO = bookDAO;
            _verseDAO = verseDAO;
        }

        /// <summary>
        /// GET: /Reference
        /// Loads the reference page with all books pre-loaded for the dropdown.
        /// </summary>
        /// <returns>Reference/Index view with book list populated.</returns>
        [HttpGet]
        public IActionResult Index(int? bookId, int? chapter)
        {
            ReferenceViewModel vm = new ReferenceViewModel
            {
                AllBooks = _bookDAO.GetAllBooks(),
                SelectedBookId = bookId,
                SelectedChapter = chapter
            };

            if (bookId.HasValue)
            {
                vm.ChapterCount = _bookDAO.GetChapterCount(bookId.Value);
            }

            return View(vm);
        }

        /// <summary>
        /// GET: /Reference/Results?bookId=43&amp;chapter=3
        /// Retrieves and displays all verses for the given book and chapter.
        /// </summary>
        /// <param name="bookId">The selected book ID from the dropdown.</param>
        /// <param name="chapter">The selected chapter number.</param>
        /// <returns>Reference/Results view with verse list.</returns>
        [HttpGet]
        public IActionResult Results(int bookId, int chapter)
        {
            // Fetch all verses for the selected book/chapter combination
            List<BibleVerse> verses = _verseDAO.GetVersesByChapter(bookId, chapter);

            // Build view model with full book list and results
            ReferenceViewModel vm = new ReferenceViewModel
            {
                AllBooks = _bookDAO.GetAllBooks(),
                SelectedBookId = bookId,
                SelectedChapter = chapter,
                ChapterCount = _bookDAO.GetChapterCount(bookId),
                Verses = verses
            };

            return View(vm);
        }
    }
}