-- ==========================================
-- Fix RefreshTokens Table - Add CreatedAt Column
-- ==========================================

USE EcommerceAPI;
GO

-- Check if CreatedAt column exists
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('RefreshTokens') 
    AND name = 'CreatedAt'
)
BEGIN
    PRINT 'Adding CreatedAt column to RefreshTokens table...';
    
    -- Add CreatedAt column with default value
    ALTER TABLE RefreshTokens
 ADD CreatedAt DATETIME2 NOT NULL 
    CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT (GETUTCDATE());
    
    PRINT 'CreatedAt column added successfully!';
END
ELSE
BEGIN
PRINT 'CreatedAt column already exists.';
END
GO

-- Verify the column was added
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RefreshTokens'
ORDER BY ORDINAL_POSITION;
GO

-- Show sample data
SELECT TOP 5 * FROM RefreshTokens ORDER BY Id DESC;
GO

PRINT 'Fix completed successfully! ?';
