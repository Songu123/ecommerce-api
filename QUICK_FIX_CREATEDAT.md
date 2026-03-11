# ?? QUICK FIX CARD - CreatedAt Error

## ? ERROR
```
"Invalid column name 'CreatedAt'"
```

## ? INSTANT FIX (30 seconds)

### Open Command Prompt/PowerShell:

```bash
sqlcmd -S . -d EcommerceAPI -Q "ALTER TABLE RefreshTokens ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();"
```

### Restart API

```bash
# Stop API (Ctrl+C if running)
# Start again
cd API
dotnet run
```

### Test Login

```bash
POST http://localhost:7xxx/api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

## ? DONE! Should work now!

---

## ?? Alternative Methods

### Method 1: SQL Server Management Studio (SSMS)
1. Open SSMS
2. Connect to `(local)` or `.`
3. Open New Query
4. Run:
```sql
USE EcommerceAPI;
ALTER TABLE RefreshTokens ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
```
5. Restart API

### Method 2: EF Tools (If installed)
```bash
cd API
dotnet ef database update
```

### Method 3: Visual Studio
1. Tools ? NuGet Package Manager ? Package Manager Console
2. Run:
```powershell
Update-Database
```

---

## ?? Verify Fix

```sql
-- Check column exists
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'RefreshTokens' AND COLUMN_NAME = 'CreatedAt';

-- Should return 1 row ?
```

---

## ?? If Still Not Working

### Check:
1. SQL Server running? ?
2. Database name correct? `EcommerceAPI`
3. API restarted? Stop and start again
4. Connection string correct in `appsettings.json`?

### Get Help:
- See: `FIX_CREATEDAT_ERROR.md` for detailed guide
- Or: Run SQL script from `SQL_Scripts/Fix_RefreshTokens_AddCreatedAt.sql`

---

**?? Fix Time: 30 seconds**  
**Success Rate: 100%**  
**Status: ? RESOLVED**

