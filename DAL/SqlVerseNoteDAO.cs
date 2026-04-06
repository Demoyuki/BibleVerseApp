// ============================================================
// File: SqlVerseNoteDAO.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Concrete ADO.NET implementation of IVerseNoteDAO.
//              The verse_notes table uses t_kjv.id as the FK,
//              which is the scrollmapper BBCCCVVV verse ID system.
// ============================================================

using System.Data.SqlClient;
using BibleVerseApp.Models;
using Microsoft.Data.SqlClient;

namespace BibleVerseApp.DAL
{
    /// <summary>
    /// SQL Server implementation of IVerseNoteDAO using ADO.NET.
    /// Persists and retrieves user notes from the verse_notes table.
    /// Foreign key verse_id references t_kjv.id (scrollmapper format).
    /// </summary>
    public class SqlVerseNoteDAO : IVerseNoteDAO
    {
        // Connection string reused across all method calls
        private readonly string _connectionString;

        /// <summary>
        /// Constructs a new SqlVerseNoteDAO.
        /// </summary>
        /// <param name="connectionString">SQL Server connection string.</param>
        public SqlVerseNoteDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all notes for a given verse ID, ordered newest first.
        /// </summary>
        /// <param name="verseId">The scrollmapper verse ID (BBCCCVVV) to fetch notes for.</param>
        /// <returns>List of VerseNote objects ordered by created_at DESC.</returns>
        /// <exception cref="Exception">Thrown on database connection failure.</exception>
        public List<VerseNote> GetNotesByVerseId(int verseId)
        {
            // Initialize list to collect all notes for this verse
            List<VerseNote> notes = new List<VerseNote>();

            string sql = @"
                SELECT note_id, verse_id, note_text, created_at
                FROM verse_notes
                WHERE verse_id = @VerseId
                ORDER BY created_at DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Parameterize verse ID to prevent SQL injection
                    cmd.Parameters.AddWithValue("@VerseId", verseId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Map each row to a VerseNote and add to list
                        while (reader.Read())
                        {
                            notes.Add(new VerseNote
                            {
                                NoteId = (int)reader["note_id"],
                                VerseId = (int)reader["verse_id"],
                                NoteText = reader["note_text"].ToString() ?? "",
                                CreatedAt = (DateTime)reader["created_at"]
                            });
                        }
                    }
                }
            }

            return notes;
        }

        /// <summary>
        /// Inserts a new note record into the verse_notes table.
        /// The note_id column is an IDENTITY and is assigned by SQL Server.
        /// </summary>
        /// <param name="note">The VerseNote to persist.</param>
        /// <returns>Number of rows affected (1 = success, 0 = failure).</returns>
        /// <exception cref="Exception">Thrown on DB failure or FK constraint violation.</exception>
        public int AddNote(VerseNote note)
        {
            string sql = @"
                INSERT INTO verse_notes (verse_id, note_text, created_at)
                VALUES (@VerseId, @NoteText, @CreatedAt)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Bind all three insert values as parameters
                    cmd.Parameters.AddWithValue("@VerseId", note.VerseId);
                    cmd.Parameters.AddWithValue("@NoteText", note.NoteText);
                    cmd.Parameters.AddWithValue("@CreatedAt", note.CreatedAt);

                    // Return row count (1 on success)
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
