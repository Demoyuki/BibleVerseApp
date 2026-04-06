// ============================================================
// File: VerseNote.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Model class representing a user-created note
//              associated with a specific Bible verse.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// Represents a user's personal note attached to a Bible verse.
    /// Maps to the verse_notes table in SQL Server.
    /// </summary>
    public class VerseNote
    {
        /// <summary>Gets or sets the primary key for the note.</summary>
        public int NoteId { get; set; }

        /// <summary>Gets or sets the foreign key linking to a BibleVerse.</summary>
        public int VerseId { get; set; }

        /// <summary>Gets or sets the text content of the note.</summary>
        public string NoteText { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when this note was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Navigation property: the verse this note is attached to.</summary>
        public BibleVerse? Verse { get; set; }
    }
}