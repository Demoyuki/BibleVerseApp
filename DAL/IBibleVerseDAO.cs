// ============================================================
// File: IBibleVerseDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Interface defining all data access operations
//              for BibleVerse entities. Enables loose coupling
//              and testability via dependency injection.
// ============================================================

using BibleVerseApp.Models;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// Defines the contract for Bible verse data access operations.
    /// Implemented by SqlBibleVerseDAO using ADO.NET against SQL Server.
    /// </summary>
    public interface IBibleVerseDAO
    {
        /// <summary>
        /// Retrieves a single Bible verse by its primary key.
        /// </summary>
        /// <param name="id">The unique verse ID.</param>
        /// <returns>BibleVerse with populated Book navigation property, or null if not found.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        BibleVerse? GetVerseById(int id);

        /// <summary>
        /// Searches all Bible verses for those containing the given search term.
        /// Filters by testament based on the boolean flags provided.
        /// </summary>
        /// <param name="term">The keyword or phrase to search for.</param>
        /// <param name="includeOT">Whether to include Old Testament results.</param>
        /// <param name="includeNT">Whether to include New Testament results.</param>
        /// <returns>List of matching BibleVerse objects ordered by book, chapter, verse.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        List<BibleVerse> SearchVerses(string term, bool includeOT, bool includeNT);

        /// <summary>
        /// Retrieves all verses from a specific book and chapter.
        /// </summary>
        /// <param name="bookId">The ID of the book to query.</param>
        /// <param name="chapter">The chapter number within the book.</param>
        /// <returns>Ordered list of BibleVerse objects for that chapter.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        List<BibleVerse> GetVersesByChapter(int bookId, int chapter);
    }
}