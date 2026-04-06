// ============================================================
// File: IVerseNoteDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Interface defining data access operations for
//              user-created VerseNote entities.
// ============================================================

using BibleVerseApp.Models;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// Defines the contract for verse note data access operations.
    /// </summary>
    public interface IVerseNoteDAO
    {
        /// <summary>
        /// Retrieves all notes associated with a specific verse.
        /// </summary>
        /// <param name="verseId">The ID of the verse to fetch notes for.</param>
        /// <returns>List of VerseNote objects ordered by creation date descending.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        List<VerseNote> GetNotesByVerseId(int verseId);

        /// <summary>
        /// Inserts a new user note into the database.
        /// </summary>
        /// <param name="note">The VerseNote object to persist. NoteId will be assigned by the database.</param>
        /// <returns>The number of rows affected (1 on success, 0 on failure).</returns>
        /// <exception cref="Exception">Thrown if database connection fails or constraint is violated.</exception>
        int AddNote(VerseNote note);
    }
}
