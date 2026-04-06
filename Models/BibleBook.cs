// ============================================================
// File: BibleBook.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Model class representing a book of the Bible,
//              including testament classification and chapter count.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// Represents one of the 66 books of the Bible.
    /// Maps to the bible_books table in the SQL Server database.
    /// </summary>
    public class BibleBook
    {
        /// <summary>Gets or sets the primary key book identifier (1-66).</summary>
        public int BookId { get; set; }

        /// <summary>Gets or sets the full book name (e.g., "Genesis").</summary>
        public string BookName { get; set; } = string.Empty;

        /// <summary>Gets or sets the 3-letter abbreviation (e.g., "Gen").</summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the testament designation.
        /// Valid values: "OT" (Old Testament) or "NT" (New Testament).
        /// </summary>
        public string Testament { get; set; } = string.Empty;

        /// <summary>Gets or sets the total number of chapters in this book.</summary>
        public int ChapterCount { get; set; }

        /// <summary>
        /// Convenience property returning true if this is an Old Testament book.
        /// </summary>
        public bool IsOldTestament => Testament == "OT";

        /// <summary>
        /// Convenience property returning true if this is a New Testament book.
        /// </summary>
        public bool IsNewTestament => Testament == "NT";
    }
}
