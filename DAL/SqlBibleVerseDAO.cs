// ============================================================
// File: SqlBibleVerseDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Concrete ADO.NET implementation of IBibleVerseDAO.
//              Uses the REAL scrollmapper/bible_databases schema:
//
//   t_kjv table columns:
//     id = verse ID (BBCCCVVV format, e.g. John 3:16 = 43003016)
//     b  = book number (1-66)
//     c  = chapter number
//     v  = verse number
//     t  = verse text
//
//   key_english table columns:
//     b = book number (PK)
//     n = book name
//     t = testament (OT/NT)
//
//   key_abbreviations_english:
//     a = abbreviation text
//     b = book number it belongs to
//     p = 1 if preferred abbreviation
// ============================================================

using System.Data.SqlClient;
using BibleVerseApp.Models;
using Microsoft.Data.SqlClient;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// SQL Server implementation of IBibleVerseDAO.
    /// Queries against the scrollmapper bible_databases schema
    /// (t_kjv, key_english, key_abbreviations_english tables).
    /// All queries use parameterized inputs to prevent SQL injection.
    /// </summary>
    public class SqlBibleVerseDAO : IBibleVerseDAO
    {
        // Stored connection string used by all query methods
        private readonly string _connectionString;

        /// <summary>
        /// Constructs a new SqlBibleVerseDAO with the given connection string.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string from appsettings.json.</param>
        public SqlBibleVerseDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves a single KJV verse by its scrollmapper verse ID (BBCCCVVV).
        /// Joins to key_english and key_abbreviations_english for book info.
        /// </summary>
        /// <param name="id">The scrollmapper verse ID (e.g. 43003016 for John 3:16).</param>
        /// <returns>Populated BibleVerse with Book, or null if not found.</returns>
        /// <exception cref="Exception">Thrown on database connection failure.</exception>
        public BibleVerse? GetVerseById(int id)
        {
            // Will remain null if no matching row is found
            BibleVerse? verse = null;

            // Join verse to book name and preferred abbreviation
            string sql = @"
                SELECT v.id, v.b, v.c, v.v, v.t,
                       k.n  AS book_name,
                       k.t  AS testament,
                       a.a  AS abbreviation
                FROM t_kjv v
                INNER JOIN key_english k
                    ON v.b = k.b
                LEFT JOIN key_abbreviations_english a
                    ON a.b = v.b AND a.p = 1
                WHERE v.id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Parameterize the verse ID to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Read single result row if it exists
                        if (reader.Read())
                            verse = MapVerseFromReader(reader);
                    }
                }
            }

            return verse;
        }

        /// <summary>
        /// Searches KJV verses for those containing the given search term.
        /// Optionally filters to Old Testament, New Testament, or both.
        /// </summary>
        /// <param name="term">The keyword or phrase to search for (LIKE match).</param>
        /// <param name="includeOT">Whether to include Old Testament results.</param>
        /// <param name="includeNT">Whether to include New Testament results.</param>
        /// <returns>List of matching BibleVerse objects ordered by book, chapter, verse.</returns>
        public List<BibleVerse> SearchVerses(string term, bool includeOT, bool includeNT)
        {
            // List to accumulate all matching results
            List<BibleVerse> results = new List<BibleVerse>();

            // Build the testament WHERE clause fragment based on checkbox state
            string testamentFilter = BuildTestamentFilter(includeOT, includeNT);

            string sql = $@"
                SELECT v.id, v.b, v.c, v.v, v.t,
                       k.n  AS book_name,
                       k.t  AS testament,
                       a.a  AS abbreviation
                FROM t_kjv v
                INNER JOIN key_english k
                    ON v.b = k.b
                LEFT JOIN key_abbreviations_english a
                    ON a.b = v.b AND a.p = 1
                WHERE v.t LIKE @Term
                  AND {testamentFilter}
                ORDER BY v.b, v.c, v.v";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Wrap term in wildcards for LIKE pattern matching
                    cmd.Parameters.AddWithValue("@Term", $"%{term}%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Add each matching verse row to the results list
                        while (reader.Read())
                            results.Add(MapVerseFromReader(reader));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves all KJV verses from a specific book and chapter.
        /// </summary>
        /// <param name="bookId">The scrollmapper book number (1-66).</param>
        /// <param name="chapter">The chapter number within the book.</param>
        /// <returns>Ordered list of BibleVerse objects for that chapter.</returns>
        public List<BibleVerse> GetVersesByChapter(int bookId, int chapter)
        {
            // Accumulate all verses found in this chapter
            List<BibleVerse> verses = new List<BibleVerse>();

            string sql = @"
                SELECT v.id, v.b, v.c, v.v, v.t,
                       k.n  AS book_name,
                       k.t  AS testament,
                       a.a  AS abbreviation
                FROM t_kjv v
                INNER JOIN key_english k
                    ON v.b = k.b
                LEFT JOIN key_abbreviations_english a
                    ON a.b = v.b AND a.p = 1
                WHERE v.b = @BookId
                  AND v.c = @Chapter
                ORDER BY v.v";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Bind both filter parameters to prevent injection
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.Parameters.AddWithValue("@Chapter", chapter);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Map every returned row to a BibleVerse object
                        while (reader.Read())
                            verses.Add(MapVerseFromReader(reader));
                    }
                }
            }

            return verses;
        }

        /// <summary>
        /// Maps the current SqlDataReader row to a BibleVerse model.
        /// Column aliases used: book_name, testament, abbreviation.
        /// </summary>
        /// <param name="reader">Open SqlDataReader positioned on a valid row.</param>
        /// <returns>Fully populated BibleVerse with nested BibleBook.</returns>
        private BibleVerse MapVerseFromReader(SqlDataReader reader)
        {
            // Map scrollmapper single-letter columns to our named model properties
            return new BibleVerse
            {
                Id       = (int)reader["id"],
                BookId   = (int)reader["b"],
                Chapter  = (int)reader["c"],
                VerseNum = (int)reader["v"],
                Text     = reader["t"].ToString() ?? "",
                Book = new BibleBook
                {
                    BookId       = (int)reader["b"],
                    BookName     = reader["book_name"].ToString() ?? "",
                    Testament    = reader["testament"].ToString() ?? "",
                    Abbreviation = reader["abbreviation"]?.ToString() ?? ""
                }
            };
        }

        /// <summary>
        /// Builds a SQL WHERE clause fragment for testament filtering.
        /// References k.t (key_english.t column aliased via join).
        /// </summary>
        /// <param name="ot">Include Old Testament books.</param>
        /// <param name="nt">Include New Testament books.</param>
        /// <returns>SQL fragment string for WHERE clause.</returns>
        private string BuildTestamentFilter(bool ot, bool nt)
        {
            if (ot && nt) return "1=1";         // Both: no filter
            if (ot)       return "k.t = 'OT'";  // Old Testament only
            if (nt)       return "k.t = 'NT'";  // New Testament only
            return "1=0";                        // Neither: empty result set
        }
    }
}
