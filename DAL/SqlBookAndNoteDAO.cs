// ============================================================
// File: SqlBibleBookDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Concrete ADO.NET implementation of IBibleBookDAO.
//              Queries the scrollmapper key_english table:
//                b = book number (PK)
//                n = full book name
//                t = testament (OT/NT)
//              And key_abbreviations_english for abbreviations:
//                a = abbreviation, b = book ref, p = preferred flag
//              Chapter count is derived from MAX(c) in t_kjv.
// ============================================================

using System.Data.SqlClient;
using BibleVerseApp.Models;
using Microsoft.Data.SqlClient;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// SQL Server implementation of IBibleBookDAO using ADO.NET.
    /// Reads book metadata from the scrollmapper key_english table.
    /// </summary>
    public class SqlBibleBookDAO : IBibleBookDAO
    {
        // Connection string stored for reuse across all method calls
        private readonly string _connectionString;

        /// <summary>
        /// Constructs a new SqlBibleBookDAO.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string from appsettings.json.</param>
        public SqlBibleBookDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Returns all 66 books ordered by canonical sequence (book number).
        /// Joins key_english to key_abbreviations_english for abbreviation,
        /// and t_kjv to derive each book's chapter count.
        /// </summary>
        /// <returns>Complete ordered list of BibleBook objects.</returns>
        /// <exception cref="Exception">Thrown on database connection failure.</exception>
        public List<BibleBook> GetAllBooks()
        {
            // Result list to populate with all 66 books
            List<BibleBook> books = new List<BibleBook>();

            // Derive chapter count using MAX(c) grouped by book number
            string sql = @"
                SELECT k.b,
                       k.n  AS book_name,
                       k.t  AS testament,
                       a.a  AS abbreviation,
                       MAX(v.c) AS chapter_count
                FROM key_english k
                LEFT JOIN key_abbreviations_english a
                    ON a.b = k.b AND a.p = 1
                LEFT JOIN t_kjv v
                    ON v.b = k.b
                GROUP BY k.b, k.n, k.t, a.a
                ORDER BY k.b";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Map each row to a BibleBook and add to the results list
                    while (reader.Read())
                    {
                        books.Add(new BibleBook
                        {
                            BookId       = (int)reader["b"],
                            BookName     = reader["book_name"].ToString() ?? "",
                            Testament    = reader["testament"].ToString() ?? "",
                            Abbreviation = reader["abbreviation"]?.ToString() ?? "",
                            // Handle potential null if t_kjv has no data yet
                            ChapterCount = reader["chapter_count"] == DBNull.Value
                                           ? 0
                                           : (int)reader["chapter_count"]
                        });
                    }
                }
            }

            return books;
        }

        /// <summary>
        /// Returns the highest chapter number for a given book,
        /// derived from MAX(c) in the t_kjv verse table.
        /// </summary>
        /// <param name="bookId">The scrollmapper book number (1-66).</param>
        /// <returns>Total chapter count, or 0 if book not found.</returns>
        public int GetChapterCount(int bookId)
        {
            // Default to 0 if no verse data is found for this book
            int count = 0;

            // Use MAX(c) to derive chapter count dynamically from t_kjv
            string sql = "SELECT MAX(c) FROM t_kjv WHERE b = @BookId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Parameterize book ID to prevent SQL injection
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    object? result = cmd.ExecuteScalar();

                    // Assign result only if a non-null value was returned
                    if (result != null && result != DBNull.Value)
                        count = (int)result;
                }
            }

            return count;
        }
    }
}