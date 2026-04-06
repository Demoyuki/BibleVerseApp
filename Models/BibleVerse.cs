// ============================================================
// File: BibleVerse.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Model class representing a single Bible verse
//              from the bible_verses database table.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// Represents a single verse from the Bible.
    /// Maps to the bible_verses table in the SQL Server database.
    /// </summary>
    public class BibleVerse
    {
        /// <summary>Gets or sets the primary key identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the foreign key referencing BibleBook.</summary>
        public int BookId { get; set; }

        /// <summary>Gets or sets the chapter number within the book.</summary>
        public int Chapter { get; set; }

        /// <summary>Gets or sets the verse number within the chapter.</summary>
        public int VerseNum { get; set; }

        /// <summary>Gets or sets the full text content of the verse (KJV).</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Navigation property: the book this verse belongs to.</summary>
        public BibleBook? Book { get; set; }

        /// <summary>
        /// Returns a formatted reference string, e.g. "Jhn 3:3".
        /// </summary>
        /// <returns>Short reference string combining book abbreviation, chapter, and verse.</returns>
        public string ShortReference =>
            $"{Book?.Abbreviation ?? "?"} {Chapter}:{VerseNum}";

        /// <summary>
        /// Returns the full display string with reference and text.
        /// </summary>
        /// <returns>Formatted string "[Jhn 3:3 KJV] verse text..."</returns>
        public string FullDisplay =>
            $"[{ShortReference} KJV] {Text}";
    }
}
