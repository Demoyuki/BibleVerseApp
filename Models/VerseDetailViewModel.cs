// ============================================================
// File: VerseDetailViewModel.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: ViewModel combining a single verse with its
//              associated notes and a new-note entry field.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// View model for the single verse detail page.
    /// Combines the verse text, existing notes, and a new note form.
    /// </summary>
    public class VerseDetailViewModel
    {
        /// <summary>Gets or sets the verse being displayed.</summary>
        public BibleVerse Verse { get; set; } = new BibleVerse();

        /// <summary>Gets or sets the list of notes saved for this verse.</summary>
        public List<VerseNote> Notes { get; set; } = new List<VerseNote>();

        /// <summary>Gets or sets the new note being composed by the user.</summary>
        public VerseNote NewNote { get; set; } = new VerseNote();
    }
}
