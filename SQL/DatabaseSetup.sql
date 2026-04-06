-- ============================================================
-- File: DatabaseSetup.sql
-- Author: Victor Marrujo
-- Course: CST-350
-- Description: Creates the verse_notes table and required
--              indexes in the BibleVerseDB database.
--              Run AFTER importing scrollmapper/bible_databases
--              MS SQL import file which creates bible_books
--              and bible_verses tables.
-- ============================================================
CREATE DATABASE BibleVerseDB;
USE BibleVerseDB;
GO

-- -------------------------------------------------------
-- Create the verse_notes table for user comments
-- -------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='verse_notes' AND xtype='U')
BEGIN
    CREATE TABLE verse_notes (
        note_id    INT IDENTITY(1,1) PRIMARY KEY,
        verse_id   INT NOT NULL,
        note_text  NVARCHAR(MAX) NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),

        -- Foreign key to the scrollmapper bible_verses table
        CONSTRAINT FK_verse_notes_verse
            FOREIGN KEY (verse_id)
            REFERENCES bible_verses(id)
            ON DELETE CASCADE
    );
    PRINT 'verse_notes table created.';
END
ELSE
BEGIN
    PRINT 'verse_notes table already exists.';
END
GO

-- -------------------------------------------------------
-- Index on verse_id for fast note lookups per verse
-- -------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_verse_notes_verse_id')
BEGIN
    CREATE INDEX IX_verse_notes_verse_id ON verse_notes(verse_id);
    PRINT 'Index IX_verse_notes_verse_id created.';
END
GO

-- -------------------------------------------------------
-- Add testament column to bible_books if not present
-- (scrollmapper schema may vary; this ensures it exists)
-- -------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'bible_books' AND COLUMN_NAME = 'testament'
)
BEGIN
    ALTER TABLE bible_books ADD testament NVARCHAR(3) NULL;

    -- Populate OT books (Genesis=1 through Malachi=39)
    UPDATE bible_books SET testament = 'OT' WHERE book_id BETWEEN 1 AND 39;

    -- Populate NT books (Matthew=40 through Revelation=66)
    UPDATE bible_books SET testament = 'NT' WHERE book_id BETWEEN 40 AND 66;

    PRINT 'testament column added and populated.';
END
GO

-- -------------------------------------------------------
-- Add abbreviation column if not already present
-- -------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'bible_books' AND COLUMN_NAME = 'abbreviation'
)
BEGIN
    ALTER TABLE bible_books ADD abbreviation NVARCHAR(10) NULL;

    -- Populate standard abbreviations for all 66 books
    UPDATE bible_books SET abbreviation = CASE book_id
        WHEN 1  THEN 'Gen'  WHEN 2  THEN 'Exo'  WHEN 3  THEN 'Lev'
        WHEN 4  THEN 'Num'  WHEN 5  THEN 'Deu'  WHEN 6  THEN 'Jos'
        WHEN 7  THEN 'Jdg'  WHEN 8  THEN 'Rut'  WHEN 9  THEN '1Sa'
        WHEN 10 THEN '2Sa'  WHEN 11 THEN '1Ki'  WHEN 12 THEN '2Ki'
        WHEN 13 THEN '1Ch'  WHEN 14 THEN '2Ch'  WHEN 15 THEN 'Ezr'
        WHEN 16 THEN 'Neh'  WHEN 17 THEN 'Est'  WHEN 18 THEN 'Job'
        WHEN 19 THEN 'Psa'  WHEN 20 THEN 'Pro'  WHEN 21 THEN 'Ecc'
        WHEN 22 THEN 'SoS'  WHEN 23 THEN 'Isa'  WHEN 24 THEN 'Jer'
        WHEN 25 THEN 'Lam'  WHEN 26 THEN 'Eze'  WHEN 27 THEN 'Dan'
        WHEN 28 THEN 'Hos'  WHEN 29 THEN 'Joe'  WHEN 30 THEN 'Amo'
        WHEN 31 THEN 'Oba'  WHEN 32 THEN 'Jon'  WHEN 33 THEN 'Mic'
        WHEN 34 THEN 'Nah'  WHEN 35 THEN 'Hab'  WHEN 36 THEN 'Zep'
        WHEN 37 THEN 'Hag'  WHEN 38 THEN 'Zec'  WHEN 39 THEN 'Mal'
        WHEN 40 THEN 'Mat'  WHEN 41 THEN 'Mrk'  WHEN 42 THEN 'Luk'
        WHEN 43 THEN 'Jhn'  WHEN 44 THEN 'Act'  WHEN 45 THEN 'Rom'
        WHEN 46 THEN '1Co'  WHEN 47 THEN '2Co'  WHEN 48 THEN 'Gal'
        WHEN 49 THEN 'Eph'  WHEN 50 THEN 'Php'  WHEN 51 THEN 'Col'
        WHEN 52 THEN '1Th'  WHEN 53 THEN '2Th'  WHEN 54 THEN '1Ti'
        WHEN 55 THEN '2Ti'  WHEN 56 THEN 'Tit'  WHEN 57 THEN 'Phm'
        WHEN 58 THEN 'Heb'  WHEN 59 THEN 'Jas'  WHEN 60 THEN '1Pe'
        WHEN 61 THEN '2Pe'  WHEN 62 THEN '1Jo'  WHEN 63 THEN '2Jo'
        WHEN 64 THEN '3Jo'  WHEN 65 THEN 'Jud'  WHEN 66 THEN 'Rev'
        ELSE 'UNK'
    END;

    PRINT 'abbreviation column added and populated.';
END
GO

PRINT 'Database setup complete.';
