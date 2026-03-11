# ?? QUICK FIX: Invalid column name 'CreatedAt' Error

## ? Error Message
```json
{
  "success": false,
  "message": "?ã x?y ra l?i không mong mu?n. Vui lòng th? l?i sau.",
  "errors": [
    "Invalid column name 'CreatedAt'."
  ]
}
```

## ?? Root Cause

B?ng `RefreshTokens` trong database **ch?a có c?t `CreatedAt`** nh?ng code ?ang c? g?ng insert/query v?i field này.

### Why?
Migration m?i (`20260310120000_AddCreatedAtToRefreshToken.cs`) ?ã ???c t?o nh?ng **ch?a apply** vào database.

---

## ? SOLUTIONS (Choose One)

### ?? Solution 1: Run SQL Script (FASTEST - 30 seconds)

#### Step 1: M? SQL Server Management Studio (SSMS)

#### Step 2: Connect to Database
- Server: `.` ho?c `localhost`
- Database: `EcommerceAPI`

#### Step 3: Run SQL Script
Copy và execute script này:

```sql
-- Add CreatedAt column to RefreshTokens table
USE EcommerceAPI;
GO

ALTER TABLE RefreshTokens
ADD CreatedAt DATETIME2 NOT NULL 
CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT (GETUTCDATE());
GO

-- Verify
SELECT * FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RefreshTokens' AND COLUMN_NAME = 'CreatedAt';
GO

PRINT 'Fix completed! ?';
```

#### Step 4: Restart API
```bash
# Stop API (Ctrl+C)
# Run again
dotnet run
```

#### Step 5: Test Login
```bash
POST /api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**? Should work now!**

---

### ??? Solution 2: Install EF Tools & Update Database (PROPER WAY - 3 minutes)

#### Step 1: Install EF Core Tools (if not installed)
```bash
dotnet tool install --global dotnet-ef

# Or update if already installed
dotnet tool update --global dotnet-ef
```

#### Step 2: Verify Installation
```bash
dotnet ef --version
```

Should show: `Entity Framework Core .NET Command-line Tools 8.x.x`

#### Step 3: Navigate to API Project
```bash
cd D:\ASP_NET_CORE_MVC\E_commerce\API
```

#### Step 4: Update Database
```bash
dotnet ef database update
```

**Output should be:**
```
Build succeeded.
Applying migration '20260310120000_AddCreatedAtToRefreshToken'.
Done.
```

#### Step 5: Restart API & Test
```bash
dotnet run

# Test login
POST /api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**? Fixed!**

---

### ?? Solution 3: Drop & Recreate Database (NUCLEAR OPTION - 2 minutes)

?? **WARNING:** This will delete ALL data!

#### Step 1: Drop Database
```bash
cd API
dotnet ef database drop --force
```

#### Step 2: Recreate Database with All Migrations
```bash
dotnet ef database update
```

#### Step 3: Run API (Auto-seed will run)
```bash
dotnet run
```

#### Step 4: Test
```bash
POST /api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**? Fixed with fresh data!**

---

### ?? Solution 4: Manual SQL in Visual Studio (if SSMS not available)

#### Step 1: Open Server Explorer in Visual Studio
- View ? Server Explorer
- Add Connection ? SQL Server

#### Step 2: Connect to Database
- Server: `.`
- Database: `EcommerceAPI`

#### Step 3: New Query
- Right-click database ? New Query

#### Step 4: Run SQL
```sql
USE EcommerceAPI;

ALTER TABLE RefreshTokens
ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();

SELECT * FROM RefreshTokens;
```

#### Step 5: Restart API
```bash
dotnet run
```

**? Fixed!**

---

## ?? Verify Fix

### Check 1: SQL Query
```sql
-- Check if CreatedAt column exists
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RefreshTokens';
```

**Expected output:**
```
COLUMN_NAME    DATA_TYPE    IS_NULLABLE
--------------------    -----------
Id    int       NO
UserId         int     NO
Token   nvarchar     NO
ExpiryDate     datetime2    NO
IsRevoked      bit   NO
CreatedAt      datetime2    NO  ? This should appear
```

### Check 2: Test Login API
```bash
POST http://localhost:7xxx/api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**Expected response:**
```json
{
  "success": true,
  "message": "??ng nh?p thành công",
  "data": {
    "userId": 1,
    "token": "eyJhbG...",
    "refreshToken": "base64_string",
    "expiresAt": "2024-03-10T..."
  }
}
```

### Check 3: Verify Token in Database
```sql
SELECT * FROM RefreshTokens WHERE UserId = 1;
```

**Expected:** Should see row with CreatedAt value

---

## ?? Recommended Solution Path

### For Quick Fix (30 seconds):
? **Use Solution 1 (SQL Script)**

### For Proper Fix (3 minutes):
? **Use Solution 2 (EF Tools)**

### If You Want Fresh Start:
? **Use Solution 3 (Drop & Recreate)**

---

## ?? Step-by-Step (Solution 2 - Recommended)

```bash
# 1. Install EF tools (one-time)
dotnet tool install --global dotnet-ef

# 2. Navigate to project
cd D:\ASP_NET_CORE_MVC\E_commerce\API

# 3. Check pending migrations
dotnet ef migrations list

# Should show:
# 20260306024031_CreateEntities (Applied)
# 20260306101427_CreateOrder (Applied)
# 20260310110727_CreateRefreshToken (Applied)
# 20260310120000_AddCreatedAtToRefreshToken (Pending) ? This needs to run

# 4. Update database
dotnet ef database update

# 5. Run API
dotnet run

# 6. Test
# Open browser: https://localhost:7xxx
# Try login endpoint
```

---

## ?? Troubleshooting

### Issue: `dotnet ef` command not found

**Solution:**
```bash
# Install globally
dotnet tool install --global dotnet-ef

# Add to PATH if needed
# Windows: Add %USERPROFILE%\.dotnet\tools to PATH
```

### Issue: Migration already applied but column missing

**Solution:**
```bash
# Check database
sqlcmd -S . -d EcommerceAPI -Q "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='RefreshTokens'"

# If CreatedAt missing, run SQL script manually
```

### Issue: Multiple migrations pending

**Solution:**
```bash
# Apply all migrations at once
dotnet ef database update
```

### Issue: Cannot connect to database

**Solution:**
```bash
# Check connection string in appsettings.json
# Verify SQL Server is running
# Test connection with SSMS
```

---

## ?? Before vs After

### Before (Broken)
```sql
SELECT * FROM RefreshTokens;

Columns:
- Id
- UserId
- Token
- ExpiryDate
- IsRevoked
? CreatedAt (MISSING!)
```

### After (Fixed)
```sql
SELECT * FROM RefreshTokens;

Columns:
- Id
- UserId
- Token
- ExpiryDate
- IsRevoked
- CreatedAt ? (ADDED!)
```

---

## ?? Prevention

### Always Run Migrations After Creating Them

```bash
# After creating migration
dotnet ef migrations add MigrationName

# IMMEDIATELY run
dotnet ef database update

# This prevents schema mismatch
```

### Check Migration Status
```bash
# List all migrations and their status
dotnet ef migrations list

# Output shows (Applied) or (Pending)
```

---

## ?? Quick Command Reference

```bash
# Install EF tools
dotnet tool install --global dotnet-ef

# Check EF version
dotnet ef --version

# List migrations
dotnet ef migrations list --project API

# Apply migrations
dotnet ef database update --project API

# Drop database (?? deletes data)
dotnet ef database drop --force --project API

# Create new migration
dotnet ef migrations add MigrationName --project API

# Remove last migration (if not applied)
dotnet ef migrations remove --project API

# Generate SQL script
dotnet ef migrations script --project API --output migration.sql
```

---

## ? Success Indicators

After applying fix:

1. ? Login API returns 200 OK
2. ? Token and RefreshToken returned
3. ? No "Invalid column name" error
4. ? RefreshTokens table has CreatedAt column
5. ? Data inserted successfully

---

## ?? Still Having Issues?

### Check These:

1. **SQL Server Running?**
   ```bash
   # Check services
   services.msc
   # Look for: SQL Server (MSSQLSERVER)
   ```

2. **Connection String Correct?**
   ```json
 // In appsettings.json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=EcommerceAPI;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

3. **Database Exists?**
```sql
   -- In SSMS
   SELECT name FROM sys.databases WHERE name = 'EcommerceAPI';
   ```

4. **Migration Files Present?**
   ```bash
   # Check files
   dir API\Migrations\*RefreshToken*
   ```

---

## ?? Once Fixed

Your login will work and return:

```json
{
  "success": true,
  "message": "??ng nh?p thành công",
  "data": {
    "userId": 1,
    "fullName": "Admin User",
    "email": "admin@electro.com",
    "role": "Admin",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "k8j7h6g5f4d3s2a1...",
    "expiresAt": "2024-03-10T11:30:00Z"
  }
}
```

And RefreshTokens table will have:
```
Id | UserId | Token | ExpiryDate | IsRevoked | CreatedAt ?
```

---

<div align="center">

**?? Choose Solution 1 (SQL Script) for fastest fix!**

**Or Solution 2 (EF Tools) for proper way!**

</div>
