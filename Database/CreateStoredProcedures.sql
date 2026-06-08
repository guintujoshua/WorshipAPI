-- ===============================================
-- Worship API - Stored Procedures
-- MySQL Stored Procedures for Users and Songs
-- ===============================================

USE WorshipDB;

DELIMITER $$

-- ===============================================
-- USER STORED PROCEDURES
-- ===============================================

-- Get all active users
DROP PROCEDURE IF EXISTS sp_GetAllUsers$$
CREATE PROCEDURE sp_GetAllUsers()
BEGIN
	SELECT Id, Name, Email, Role, HashedPassword, Status 
	FROM Users 
	WHERE Status != 'Deleted'
	ORDER BY Id;
END$$

-- Get user by ID
DROP PROCEDURE IF EXISTS sp_GetUserById$$
CREATE PROCEDURE sp_GetUserById(
	IN p_Id INT
)
BEGIN
	SELECT Id, Name, Email, Role, HashedPassword, Status 
	FROM Users 
	WHERE Id = p_Id AND Status != 'Deleted';
END$$

-- Create new user
DROP PROCEDURE IF EXISTS sp_CreateUser$$
CREATE PROCEDURE sp_CreateUser(
	IN p_Name VARCHAR(255),
	IN p_Email VARCHAR(255),
	IN p_Role VARCHAR(100),
	IN p_HashedPassword VARCHAR(255),
	IN p_Status VARCHAR(50)
)
BEGIN
	INSERT INTO Users (Name, Email, Role, HashedPassword, Status) 
	VALUES (p_Name, p_Email, p_Role, p_HashedPassword, p_Status);

	SELECT LAST_INSERT_ID() AS Id;
END$$

-- Update user (does NOT update status)
DROP PROCEDURE IF EXISTS sp_UpdateUser$$
CREATE PROCEDURE sp_UpdateUser(
	IN p_Id INT,
	IN p_Name VARCHAR(255),
	IN p_Email VARCHAR(255),
	IN p_Role VARCHAR(100),
	IN p_HashedPassword VARCHAR(255)
)
BEGIN
	UPDATE Users 
	SET Name = p_Name,
		Email = p_Email,
		Role = p_Role,
		HashedPassword = p_HashedPassword,
		UpdatedAt = CURRENT_TIMESTAMP
	WHERE Id = p_Id AND Status != 'Deleted';

	SELECT ROW_COUNT() AS AffectedRows;
END$$

-- Soft delete user (updates status to 'Deleted')
DROP PROCEDURE IF EXISTS sp_DeleteUser$$
CREATE PROCEDURE sp_DeleteUser(
	IN p_Id INT
)
BEGIN
	UPDATE Users 
	SET Status = 'Deleted',
		UpdatedAt = CURRENT_TIMESTAMP
	WHERE Id = p_Id;

	SELECT ROW_COUNT() AS AffectedRows;
END$$

-- ===============================================
-- SONG STORED PROCEDURES
-- ===============================================

-- Get all active songs
DROP PROCEDURE IF EXISTS sp_GetAllSongs$$
CREATE PROCEDURE sp_GetAllSongs()
BEGIN
	SELECT Id, Title, Artist, `Key`, Year, LyricsWithChords, Status 
	FROM Songs 
	WHERE Status = TRUE
	ORDER BY Id;
END$$

-- Get song by ID
DROP PROCEDURE IF EXISTS sp_GetSongById$$
CREATE PROCEDURE sp_GetSongById(
	IN p_Id INT
)
BEGIN
	SELECT Id, Title, Artist, `Key`, Year, LyricsWithChords, Status 
	FROM Songs 
	WHERE Id = p_Id AND Status = TRUE;
END$$

-- Create new song
DROP PROCEDURE IF EXISTS sp_CreateSong$$
CREATE PROCEDURE sp_CreateSong(
	IN p_Title VARCHAR(255),
	IN p_Artist VARCHAR(255),
	IN p_Key CHAR(2),
	IN p_Year VARCHAR(4),
	IN p_LyricsWithChords JSON,
	IN p_Status BOOLEAN
)
BEGIN
	INSERT INTO Songs (Title, Artist, `Key`, Year, LyricsWithChords, Status) 
	VALUES (p_Title, p_Artist, p_Key, p_Year, p_LyricsWithChords, p_Status);

	SELECT LAST_INSERT_ID() AS Id;
END$$

-- Update song (does NOT update status)
DROP PROCEDURE IF EXISTS sp_UpdateSong$$
CREATE PROCEDURE sp_UpdateSong(
	IN p_Id INT,
	IN p_Title VARCHAR(255),
	IN p_Artist VARCHAR(255),
	IN p_Key CHAR(2),
	IN p_Year VARCHAR(4),
	IN p_LyricsWithChords JSON
)
BEGIN
	UPDATE Songs 
	SET Title = p_Title,
		Artist = p_Artist,
		`Key` = p_Key,
		Year = p_Year,
		LyricsWithChords = p_LyricsWithChords,
		UpdatedAt = CURRENT_TIMESTAMP
	WHERE Id = p_Id AND Status = TRUE;

	SELECT ROW_COUNT() AS AffectedRows;
END$$

-- Soft delete song (updates status to FALSE)
DROP PROCEDURE IF EXISTS sp_DeleteSong$$
CREATE PROCEDURE sp_DeleteSong(
	IN p_Id INT
)
BEGIN
	UPDATE Songs 
	SET Status = FALSE,
		UpdatedAt = CURRENT_TIMESTAMP
	WHERE Id = p_Id;

	SELECT ROW_COUNT() AS AffectedRows;
END$$

DELIMITER ;

-- ===============================================
-- Verify stored procedures were created
-- ===============================================
SELECT 
	ROUTINE_NAME as 'Stored Procedure',
	ROUTINE_TYPE as 'Type',
	CREATED as 'Created Date'
FROM 
	information_schema.ROUTINES 
WHERE 
	ROUTINE_SCHEMA = 'WorshipDB'
	AND ROUTINE_TYPE = 'PROCEDURE'
ORDER BY 
	ROUTINE_NAME;
