// ============================================================
// File: IBibleBookDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Interface defining data access operations for
//              BibleBook entities.
// ============================================================

using BibleVerseApp.Models;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// Defines the contract for Bible book data access operations.
    /// </summary>
    public interface IBibleBookDAO
    {
        /// <summary>
        /// Retrieves all 66 books of the Bible ordered by their standard canonical order.
        /// </summary>
        /// <returns>Full list of BibleBook objects.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        List<BibleBook> GetAllBooks();

        /// <summary>
        /// Gets the total number of chapters for a given book.
        /// </summary>
        /// <param name="bookId">The primary key of the book.</param>
        /// <returns>Integer chapter count, or 0 if not found.</returns>
        /// <exception cref="Exception">Thrown if database connection fails.</exception>
        int GetChapterCount(int bookId);
    }
}