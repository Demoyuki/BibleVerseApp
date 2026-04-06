-- ============================================================
-- File: DatabaseSetup_Scrollmapper.sql
-- Author: Victor Marrujo
-- Course: CST-350
-- Description: Converts the scrollmapper MySQL bible_databases
--              schema into SQL Server-compatible tables and
--              creates the verse_notes table for user comments.
--
-- REAL TABLE STRUCTURE from scrollmapper/bible_databases:
--
--   key_english        -> book lookup (b=bookNum, n=name, t=OT/NT)
--   key_abbreviations_english -> abbreviations (a=abbrev, b=bookId, p=preferred)
--   t_kjv              -> KJV verse table (id=verseId, b=book, c=chapter, v=verse, t=text)
--   bible_version_key  -> translation metadata
--
-- VERSE ID SYSTEM: id = BOOK(2 digits) + CHAPTER(3 digits) + VERSE(3 digits)
--   e.g. John 3:16 = 43003016
-- ============================================================

USE BibleVerseDB;
GO

-- ============================================================
-- STEP 1: Create SQL Server versions of the scrollmapper tables
--         (Run this BEFORE importing the MySQL dump)
-- ============================================================

-- Book lookup table (mirrors key_english from MySQL dump)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='key_english' AND xtype='U')
BEGIN
    CREATE TABLE key_english (
        b   INT           NOT NULL PRIMARY KEY,   -- Book number (1-66)
        n   NVARCHAR(50)  NOT NULL,               -- Full book name
        t   VARCHAR(2)    NOT NULL,               -- Testament: OT or NT
        g   TINYINT       NOT NULL DEFAULT 0      -- Genre ID
    );
    PRINT 'key_english created.';
END
GO

-- Abbreviations table (mirrors key_abbreviations_english)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='key_abbreviations_english' AND xtype='U')
BEGIN
    CREATE TABLE key_abbreviations_english (
        id  SMALLINT      NOT NULL PRIMARY KEY,   -- Abbreviation ID
        a   NVARCHAR(255) NOT NULL,               -- Abbreviation text
        b   SMALLINT      NOT NULL,               -- Book ID this refers to
        p   TINYINT       NOT NULL DEFAULT 0      -- 1 = preferred abbreviation
    );
    PRINT 'key_abbreviations_english created.';
END
GO

-- KJV verse table (mirrors t_kjv)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='t_kjv' AND xtype='U')
BEGIN
    CREATE TABLE t_kjv (
        id  INT           NOT NULL PRIMARY KEY,   -- Verse ID (BBCCCVVV format)
        b   INT           NOT NULL,               -- Book number
        c   INT           NOT NULL,               -- Chapter number
        v   INT           NOT NULL,               -- Verse number
        t   NVARCHAR(MAX) NOT NULL                -- Verse text
    );
    CREATE INDEX IX_t_kjv_book_chapter ON t_kjv(b, c);
    PRINT 't_kjv created with index.';
END
GO

-- ============================================================
-- STEP 2: Create verse_notes table for user comments
-- ============================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='verse_notes' AND xtype='U')
BEGIN
    CREATE TABLE verse_notes (
        note_id    INT IDENTITY(1,1) PRIMARY KEY,
        verse_id   INT           NOT NULL,        -- FK to t_kjv.id
        note_text  NVARCHAR(MAX) NOT NULL,
        created_at DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_verse_notes_verse
            FOREIGN KEY (verse_id) REFERENCES t_kjv(id) ON DELETE CASCADE
    );
    CREATE INDEX IX_verse_notes_verse_id ON verse_notes(verse_id);
    PRINT 'verse_notes created.';
END
GO

-- ============================================================
-- STEP 3: Populate key_english with all 66 books
--         (Only needed if importing manually instead of from dump)
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM key_english)
BEGIN
    INSERT INTO key_english (b, n, t, g) VALUES
    (1,'Genesis','OT',1),(2,'Exodus','OT',1),(3,'Leviticus','OT',1),
    (4,'Numbers','OT',1),(5,'Deuteronomy','OT',1),(6,'Joshua','OT',2),
    (7,'Judges','OT',2),(8,'Ruth','OT',2),(9,'1 Samuel','OT',2),
    (10,'2 Samuel','OT',2),(11,'1 Kings','OT',2),(12,'2 Kings','OT',2),
    (13,'1 Chronicles','OT',2),(14,'2 Chronicles','OT',2),(15,'Ezra','OT',2),
    (16,'Nehemiah','OT',2),(17,'Esther','OT',2),(18,'Job','OT',3),
    (19,'Psalms','OT',3),(20,'Proverbs','OT',3),(21,'Ecclesiastes','OT',3),
    (22,'Song of Solomon','OT',3),(23,'Isaiah','OT',4),(24,'Jeremiah','OT',4),
    (25,'Lamentations','OT',4),(26,'Ezekiel','OT',4),(27,'Daniel','OT',4),
    (28,'Hosea','OT',4),(29,'Joel','OT',4),(30,'Amos','OT',4),
    (31,'Obadiah','OT',4),(32,'Jonah','OT',4),(33,'Micah','OT',4),
    (34,'Nahum','OT',4),(35,'Habakkuk','OT',4),(36,'Zephaniah','OT',4),
    (37,'Haggai','OT',4),(38,'Zechariah','OT',4),(39,'Malachi','OT',4),
    (40,'Matthew','NT',5),(41,'Mark','NT',5),(42,'Luke','NT',5),
    (43,'John','NT',5),(44,'Acts','NT',5),(45,'Romans','NT',6),
    (46,'1 Corinthians','NT',6),(47,'2 Corinthians','NT',6),(48,'Galatians','NT',6),
    (49,'Ephesians','NT',6),(50,'Philippians','NT',6),(51,'Colossians','NT',6),
    (52,'1 Thessalonians','NT',6),(53,'2 Thessalonians','NT',6),(54,'1 Timothy','NT',6),
    (55,'2 Timothy','NT',6),(56,'Titus','NT',6),(57,'Philemon','NT',6),
    (58,'Hebrews','NT',6),(59,'James','NT',6),(60,'1 Peter','NT',6),
    (61,'2 Peter','NT',6),(62,'1 John','NT',6),(63,'2 John','NT',6),
    (64,'3 John','NT',6),(65,'Jude','NT',6),(66,'Revelation','NT',7);
    PRINT 'key_english populated with 66 books.';
END
GO

-- ============================================================
-- STEP 4: Populate preferred abbreviations
-- ============================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM key_abbreviations_english WHERE p = 1)
BEGIN
    INSERT INTO key_abbreviations_english (id, a, b, p) VALUES
    (1,'Gen',1,1),(2,'Exo',2,1),(3,'Lev',3,1),(4,'Num',4,1),(5,'Deu',5,1),
    (6,'Jos',6,1),(7,'Jdg',7,1),(8,'Rut',8,1),(9,'1Sa',9,1),(10,'2Sa',10,1),
    (11,'1Ki',11,1),(12,'2Ki',12,1),(13,'1Ch',13,1),(14,'2Ch',14,1),(15,'Ezr',15,1),
    (16,'Neh',16,1),(17,'Est',17,1),(18,'Job',18,1),(19,'Psa',19,1),(20,'Pro',20,1),
    (21,'Ecc',21,1),(22,'SoS',22,1),(23,'Isa',23,1),(24,'Jer',24,1),(25,'Lam',25,1),
    (26,'Eze',26,1),(27,'Dan',27,1),(28,'Hos',28,1),(29,'Joe',29,1),(30,'Amo',30,1),
    (31,'Oba',31,1),(32,'Jon',32,1),(33,'Mic',33,1),(34,'Nah',34,1),(35,'Hab',35,1),
    (36,'Zep',36,1),(37,'Hag',37,1),(38,'Zec',38,1),(39,'Mal',39,1),
    (40,'Mat',40,1),(41,'Mrk',41,1),(42,'Luk',42,1),(43,'Jhn',43,1),(44,'Act',44,1),
    (45,'Rom',45,1),(46,'1Co',46,1),(47,'2Co',47,1),(48,'Gal',48,1),(49,'Eph',49,1),
    (50,'Php',50,1),(51,'Col',51,1),(52,'1Th',52,1),(53,'2Th',53,1),(54,'1Ti',54,1),
    (55,'2Ti',55,1),(56,'Tit',56,1),(57,'Phm',57,1),(58,'Heb',58,1),(59,'Jas',59,1),
    (60,'1Pe',60,1),(61,'2Pe',61,1),(62,'1Jo',62,1),(63,'2Jo',63,1),(64,'3Jo',64,1),
    (65,'Jud',65,1),(66,'Rev',66,1);
    PRINT 'key_abbreviations_english populated.';
END
GO

PRINT 'All setup complete. Now import the t_kjv data from bible-mysql.sql.';
GO
