# ?? JWT Refresh Token Implementation Guide

## ?? Overview

Complete implementation of JWT Refresh Token mechanism for secure authentication in ASP.NET Core 8.0 API.

---

## ?? What is Refresh Token?

### Problem
- JWT Access Tokens have short expiry (15-60 minutes) for security
- Users don't want to re-login frequently
- Need balance between security and user experience

### Solution
- **Access Token**: Short-lived (15 mins), used for API authentication
- **Refresh Token**: Long-lived (7 days), used to get new access token
- When access token expires, use refresh token to get new one

---

## ??? Implementation Details

### 1?? Database Schema

#### RefreshToken Table
```sql
CREATE TABLE RefreshTokens (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiryDate DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) 
        REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
```

### 2?? Model

```csharp
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
```

### 3?? Repository Methods

```csharp
public interface IUserRepository
{
    // Save new refresh token (revokes old ones)
    Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate);
  
    // Get active refresh token for user
    Task<RefreshToken?> GetRefreshTokenAsync(int userId);
    
    // Revoke specific user's token
    Task RevokeRefreshTokenAsync(int userId);
    
    // Revoke all tokens for user (for logout all devices)
    Task RevokeAllUserRefreshTokensAsync(int userId);
}
```

#### Implementation Logic

**SaveRefreshTokenAsync:**
- Revokes all existing active tokens for user
- Creates new refresh token
- Saves to database
- Why? Ensure one active token per user (can be modified for multi-device)

**GetRefreshTokenAsync:**
- Returns active (not revoked, not expired) token
- Orders by CreatedAt descending (latest first)

---

## ?? Authentication Flow

### Login Flow

```
User Login
    ?
Validate Credentials
    ?
Generate Access Token (15 mins)
    ?
Generate Refresh Token (7 days)
    ?
Save Refresh Token to DB
    ?
Return both tokens to client
```

### Refresh Token Flow

```
Access Token Expired
    ?
Client sends: AccessToken + RefreshToken
    ?
Validate Refresh Token from DB
    ?
Check: Not revoked, Not expired, Token matches
    ?
Generate New Access Token
    ?
Generate New Refresh Token
    ?
Revoke Old Refresh Token
    ?
Save New Refresh Token
    ?
Return new tokens
```

---

## ?? API Endpoints

### 1. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password@123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "??ng nh?p thành công",
  "data": {
    "userId": 1,
    "fullName": "John Doe",
    "email": "user@example.com",
    "role": "Customer",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64_encoded_random_bytes",
    "expiresAt": "2024-03-10T11:30:00Z"
  }
}
```

**Client should store:**
- `token` (Access Token) - in memory or sessionStorage
- `refreshToken` - in httpOnly cookie or secure storage

### 2. Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "previous_refresh_token"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Token ?ã ???c làm m?i",
  "data": {
    "userId": 1,
    "fullName": "John Doe",
    "email": "user@example.com",
    "role": "Customer",
    "token": "new_access_token",
    "refreshToken": "new_refresh_token",
    "expiresAt": "2024-03-10T12:00:00Z"
  }
}
```

---

## ?? Client-Side Implementation

### JavaScript/TypeScript Example

```typescript
class AuthService {
    private accessToken: string | null = null;
    private refreshToken: string | null = null;
    
    async login(email: string, password: string) {
        const response = await fetch('/api/auth/login', {
         method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        
        const data = await response.json();
        
        if (data.success) {
this.accessToken = data.data.token;
        this.refreshToken = data.data.refreshToken;
            
            // Store refresh token securely
        localStorage.setItem('refreshToken', this.refreshToken);
     }
        
        return data;
    }
    
    async refreshAccessToken() {
        const response = await fetch('/api/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
   body: JSON.stringify({
       accessToken: this.accessToken,
        refreshToken: this.refreshToken
            })
        });
        
    const data = await response.json();
        
        if (data.success) {
         this.accessToken = data.data.token;
            this.refreshToken = data.data.refreshToken;
localStorage.setItem('refreshToken', this.refreshToken);
     }
        
  return data;
  }
    
    async apiCall(url: string, options: RequestInit = {}) {
        // Add access token to request
        options.headers = {
  ...options.headers,
    'Authorization': `Bearer ${this.accessToken}`
        };
        
        let response = await fetch(url, options);
        
   // If 401 Unauthorized, try refresh
        if (response.status === 401) {
      await this.refreshAccessToken();
     
            // Retry original request with new token
          options.headers = {
           ...options.headers,
       'Authorization': `Bearer ${this.accessToken}`
            };
    response = await fetch(url, options);
        }
        
    return response;
  }
}
```

### React Hook Example

```typescript
import { useState, useEffect } from 'react';

export const useAuth = () => {
    const [accessToken, setAccessToken] = useState<string | null>(null);
    const [refreshToken, setRefreshToken] = useState<string | null>(null);
    
    useEffect(() => {
        // Load refresh token from storage
        const stored = localStorage.getItem('refreshToken');
   if (stored) {
     setRefreshToken(stored);
        // Try to refresh access token
     refreshAccessToken(stored);
        }
    }, []);
    
    const login = async (email: string, password: string) => {
        const response = await fetch('/api/auth/login', {
        method: 'POST',
      headers: { 'Content-Type': 'application/json' },
 body: JSON.stringify({ email, password })
        });
        
        const data = await response.json();
      
        if (data.success) {
        setAccessToken(data.data.token);
    setRefreshToken(data.data.refreshToken);
            localStorage.setItem('refreshToken', data.data.refreshToken);
      }
        
        return data;
    };
    
    const refreshAccessToken = async (token?: string) => {
        const tokenToUse = token || refreshToken;
        
        if (!tokenToUse) return;
        
        const response = await fetch('/api/auth/refresh', {
            method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
      accessToken,
   refreshToken: tokenToUse
            })
  });
        
        const data = await response.json();
        
  if (data.success) {
            setAccessToken(data.data.token);
      setRefreshToken(data.data.refreshToken);
          localStorage.setItem('refreshToken', data.data.refreshToken);
      }
    };
    
    const logout = () => {
        setAccessToken(null);
        setRefreshToken(null);
        localStorage.removeItem('refreshToken');
    };
    
    return { accessToken, refreshToken, login, refreshAccessToken, logout };
};
```

---

## ?? Security Best Practices

### ? Do's

1. **Use HTTPS**
   - Always use HTTPS in production
   - Prevents token interception

2. **Short Access Token Lifetime**
   - 15-30 minutes recommended
   - Reduces risk if compromised

3. **Longer Refresh Token Lifetime**
   - 7-30 days recommended
   - Balance between security and UX

4. **Store Securely**
   - Access Token: Memory or sessionStorage
   - Refresh Token: httpOnly cookie (best) or secure storage

5. **Rotate Refresh Tokens**
   - Generate new refresh token on each refresh
 - Revoke old refresh token

6. **Revoke on Suspicious Activity**
   - Revoke all tokens on password change
   - Revoke on logout
   - Revoke on suspicious activity detection

7. **Add Refresh Token Jti (JWT ID)**
   - Can add unique ID to track tokens
   - Useful for audit logs

### ? Don'ts

1. **Don't Store Access Token in localStorage**
   - Vulnerable to XSS attacks
   - Use memory or sessionStorage

2. **Don't Use Long-Lived Access Tokens**
   - Defeats purpose of security
   - Keep them short (15-30 mins)

3. **Don't Reuse Refresh Tokens**
   - Always rotate on refresh
   - Prevents token replay attacks

4. **Don't Skip Token Validation**
   - Always validate in database
   - Check expiry and revoked status

5. **Don't Log Tokens**
   - Never log tokens in plain text
   - Security risk if logs compromised

---

## ?? Testing

### Test Login & Get Tokens

```http
POST http://localhost:7123/api/auth/login
Content-Type: application/json

{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**Expected:**
- ? 200 OK
- ? Returns access token (JWT)
- ? Returns refresh token (base64)
- ? ExpiresAt is 15 minutes in future
- ? Token saved in database

**Verify in Database:**
```sql
SELECT * FROM RefreshTokens WHERE UserId = 1
```

### Test Refresh Token

**Step 1:** Get tokens from login

**Step 2:** Wait for access token to expire (or use expired token)

**Step 3:** Call refresh endpoint
```http
POST http://localhost:7123/api/auth/refresh
Content-Type: application/json

{
  "accessToken": "old_access_token",
  "refreshToken": "old_refresh_token"
}
```

**Expected:**
- ? 200 OK
- ? New access token returned
- ? New refresh token returned
- ? Old refresh token revoked in DB
- ? New token saved in DB

### Test Invalid Refresh Token

```http
POST http://localhost:7123/api/auth/refresh
Content-Type: application/json

{
  "accessToken": "some_token",
  "refreshToken": "invalid_token"
}
```

**Expected:**
- ? 400 Bad Request
- ? Error: "Invalid refresh token"

### Test Expired Refresh Token

**Step 1:** Manually set expiry date to past in database
```sql
UPDATE RefreshTokens 
SET ExpiryDate = DATEADD(DAY, -1, GETUTCDATE())
WHERE UserId = 1
```

**Step 2:** Try to refresh

**Expected:**
- ? 400 Bad Request
- ? Error: "Refresh token has expired"

### Test Revoked Token

**Step 1:** Revoke token
```sql
UPDATE RefreshTokens 
SET IsRevoked = 1
WHERE UserId = 1
```

**Step 2:** Try to refresh

**Expected:**
- ? 400 Bad Request
- ? Error: "Refresh token has been revoked"

---

## ?? Token Rotation Strategy

### Current Implementation
- **One Active Token Per User**
- Old token revoked when new one created
- Simple, secure for single device

### Alternative: Multi-Device Support

If you want users to stay logged in on multiple devices:

```csharp
public async Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate)
{
    // DON'T revoke existing tokens
    // Just add new one
    
    var newToken = new RefreshToken
    {
  UserId = userId,
        Token = refreshToken,
        ExpiryDate = expiryDate,
        IsRevoked = false,
        CreatedAt = DateTime.UtcNow
    };

    _appContext.RefreshTokens.Add(newToken);
    await _appContext.SaveChangesAsync();
}

// Add method to get specific token (not just latest)
public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
{
    return await _appContext.RefreshTokens
        .FirstOrDefaultAsync(rt => rt.Token == token && 
        !rt.IsRevoked && 
  rt.ExpiryDate > DateTime.UtcNow);
}
```

---

## ?? Database Queries

### Get All Active Tokens for User
```sql
SELECT * FROM RefreshTokens
WHERE UserId = 1 
  AND IsRevoked = 0
AND ExpiryDate > GETUTCDATE()
```

### Revoke All Tokens for User (Logout All Devices)
```sql
UPDATE RefreshTokens
SET IsRevoked = 1
WHERE UserId = 1
  AND IsRevoked = 0
```

### Clean Up Expired Tokens (Maintenance)
```sql
DELETE FROM RefreshTokens
WHERE ExpiryDate < DATEADD(DAY, -30, GETUTCDATE())
```

### Token Statistics
```sql
SELECT 
    COUNT(*) as TotalTokens,
    SUM(CASE WHEN IsRevoked = 0 THEN 1 ELSE 0 END) as ActiveTokens,
    SUM(CASE WHEN IsRevoked = 1 THEN 1 ELSE 0 END) as RevokedTokens,
    SUM(CASE WHEN ExpiryDate < GETUTCDATE() THEN 1 ELSE 0 END) as ExpiredTokens
FROM RefreshTokens
```

---

## ?? Common Scenarios

### Scenario 1: User Login
```
1. User enters credentials
2. Server validates
3. Server generates access + refresh token
4. Server saves refresh token to DB
5. Client receives both tokens
6. Client stores tokens securely
```

### Scenario 2: API Call with Valid Token
```
1. Client adds access token to request header
2. Server validates token
3. Server processes request
4. Server returns response
```

### Scenario 3: API Call with Expired Token
```
1. Client adds expired access token to header
2. Server detects expired token
3. Server returns 401 Unauthorized
4. Client detects 401
5. Client calls refresh endpoint with refresh token
6. Server validates refresh token from DB
7. Server generates new tokens
8. Server revokes old refresh token
9. Server saves new refresh token
10. Client receives new tokens
11. Client retries original request with new access token
12. Server processes request successfully
```

### Scenario 4: User Logout
```
1. User clicks logout
2. Client calls logout endpoint (optional)
3. Server revokes refresh token in DB
4. Client deletes stored tokens
5. User logged out
```

### Scenario 5: Suspicious Activity Detected
```
1. Server detects suspicious activity
2. Server revokes all refresh tokens for user
3. User must re-login on all devices
```

---

## ?? Advanced Features (Optional)

### 1. Token Fingerprinting
```csharp
// Add device fingerprint to refresh token
public class RefreshToken
{
    // ...existing fields...
    public string? DeviceFingerprint { get; set; }
}

// Validate fingerprint on refresh
if (storedToken.DeviceFingerprint != currentFingerprint)
{
    throw new SecurityTokenException("Token used from different device");
}
```

### 2. Token Family (Detect Token Reuse)
```csharp
public class RefreshToken
{
    // ...existing fields...
 public string? TokenFamily { get; set; } // Same for all rotations
}

// If old token used, revoke entire family
if (usedTokenDetected)
{
    RevokeTokenFamily(token.TokenFamily);
}
```

### 3. Audit Logging
```csharp
public class RefreshTokenAudit
{
    public int Id { get; set; }
    public int UserId { get; set; }
 public string Action { get; set; } // Created, Used, Revoked
    public string Token { get; set; }
    public string? IpAddress { get; set; }
  public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## ?? References

### Documentation
- [JWT.io](https://jwt.io/)
- [RFC 6749 - OAuth 2.0](https://tools.ietf.org/html/rfc6749)
- [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)

### Best Practices
- [Auth0 - Refresh Tokens](https://auth0.com/docs/secure/tokens/refresh-tokens)
- [Microsoft - Token Lifetime](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-configurable-token-lifetimes)

---

## ? Implementation Checklist

- [x] RefreshToken model created
- [x] Database migration applied
- [x] Repository methods implemented
- [x] AuthService login saves refresh token
- [x] AuthService register saves refresh token
- [x] AuthService refresh token logic implemented
- [x] Controller refresh endpoint created
- [x] Token validation added
- [x] Error handling implemented
- [x] Security measures applied
- [x] Documentation completed
- [ ] Unit tests (optional)
- [ ] Integration tests (optional)

---

## ?? Summary

You now have a **complete, secure JWT Refresh Token implementation** with:

? **Access Token** (15 mins) for API calls  
? **Refresh Token** (7 days) for token renewal  
? **Database storage** for token management  
? **Token rotation** for security  
? **Revocation support** for logout/security  
? **Error handling** for invalid/expired tokens  
? **Best practices** implementation  

**Ready for production use! ??**

---

<div align="center">

**Secure Authentication Implemented! ??**

[Back to README](./README.md)

</div>
