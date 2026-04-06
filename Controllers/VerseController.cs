// ============================================================
// File: VerseController.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: MVC Controller for displaying individual Bible
//              verses and managing associated user notes.
// ============================================================

using BibleVerseApp.DAL;
using BibleVerseApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibleVerseApp.Controllers
{
    /// <summary>
    /// Handles the Single Verse detail page including note creation.
    /// </summary>
    public class VerseController : Controller
    {
        // DAO for retrieving individual verse data
        private readonly IBibleVerseDAO _verseDAO;

        // DAO for reading and writing user notes
        private readonly IVerseNoteDAO _noteDAO;

        /// <summary>
        /// Injects verse and note DAO dependencies via constructor.
        /// </summary>
        /// <param name="verseDAO">Verse data access object.</param>
        /// <param name="noteDAO">Note data access object.</param>
        public VerseController(IBibleVerseDAO verseDAO, IVerseNoteDAO noteDAO)
        {
            _verseDAO = verseDAO;
            _noteDAO = noteDAO;
        }

        /// <summary>
        /// GET: /Verse/Details/{id}
        /// Displays a single verse and all its saved notes.
        /// </summary>
        /// <param name="id">Primary key of the verse to display.</param>
        /// <returns>Verse/Details view or 404 if verse not found.</returns>
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Attempt to retrieve the verse from the database
            BibleVerse? verse = _verseDAO.GetVerseById(id);

            // Return 404 if no matching verse exists
            if (verse == null)
                return NotFound();

            // Build detail view model with verse and its notes
            VerseDetailViewModel vm = new VerseDetailViewModel
            {
                Verse = verse,
                Notes = _noteDAO.GetNotesByVerseId(id),
                NewNote = new VerseNote { VerseId = id }
            };

            return View(vm);
        }

        /// <summary>
        /// POST: /Verse/AddNote
        /// Saves a new note for the specified verse and redirects to Details.
        /// </summary>
        /// <param name="note">VerseNote model bound from the form POST.</param>
        /// <returns>Redirect to Details page for the same verse.</returns>
        [HttpPost]
        public IActionResult AddNote(VerseDetailViewModel model)
        {
            var note = model.NewNote;

            if (!string.IsNullOrWhiteSpace(note.NoteText))
            {
                note.CreatedAt = DateTime.UtcNow;
                _noteDAO.AddNote(note);
            }

            return RedirectToAction("Details", new { id = note.VerseId });
        }
    }
}