
CREATE TABLE IF NOT EXISTS Users (
	Id INT PRIMARY KEY AUTO_INCREMENT,
	Name VARCHAR(255) NOT NULL,
	Email VARCHAR(255) NOT NULL UNIQUE,
	Role VARCHAR(100) NOT NULL,
	HashedPassword VARCHAR(255) NOT NULL,
	Status VARCHAR(50) NOT NULL,
	CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	INDEX idx_email (Email),
	INDEX idx_role (Role),
	INDEX idx_status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===============================================
-- Songs Table
-- ===============================================
CREATE TABLE IF NOT EXISTS Songs (
	Id INT PRIMARY KEY AUTO_INCREMENT,
	Title VARCHAR(255) NOT NULL,
	Artist VARCHAR(255) NOT NULL,
	`Key` CHAR(2) NOT NULL COMMENT 'Musical key (e.g., C, D, Em, F#)',
	Year VARCHAR(4) NOT NULL,
	LyricsWithChords JSON NOT NULL COMMENT 'Array of strings containing lyrics and chords',
	Status BOOLEAN NOT NULL DEFAULT TRUE,
	CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	INDEX idx_title (Title),
	INDEX idx_artist (Artist),
	INDEX idx_status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

