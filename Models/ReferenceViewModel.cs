// ============================================================
// File: ReferenceViewModel.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: ViewModel carrying book/chapter selection data
//              and the resulting list of verses for that chapter.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// View model for the Reference lookup feature (Book + Chapter selection).
    /// </summary>
    public class ReferenceViewModel
    {
        /// <summary>Gets or sets all books used to populate the Book drop-down.</summary>
        public List<BibleBook> AllBooks { get; set; } = new List<BibleBook>();

        /// <summary>Gets or sets the currently selected book ID.</summary>
        public int? SelectedBookId { get; set; }

        /// <summary>Gets or sets the currently selected chapter number.</summary>
        public int? SelectedChapter { get; set; }

        /// <summary>Gets or sets the total number of chapters for the selected book.</summary>
        public int ChapterCount { get; set; }

        /// <summary>Gets or sets the list of verses returned for the selected chapter.</summary>
        public List<BibleVerse> Verses { get; set; } = new List<BibleVerse>();
    }
}
