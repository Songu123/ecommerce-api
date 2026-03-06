# ?? API Testing Guide

Complete guide for testing E-commerce API endpoints.

---

## ?? Table of Contents

- [Testing Tools](#-testing-tools)
- [Environment Setup](#-environment-setup)
- [Authentication Testing](#-authentication-testing)
- [Product Testing](#-product-testing)
- [Category Testing](#-category-testing)
- [Order Testing](#-order-testing)
- [Error Handling Testing](#-error-handling-testing)
- [Performance Testing](#-performance-testing)

---

## ??? Testing Tools

### 1. Swagger UI (Built-in)
- **URL:** `https://localhost:7xxx`
- **Best for:** Quick testing, exploration
- **Pros:** Interactive, no setup needed
- **Cons:** Limited automation

### 2. Postman
- **Download:** https://www.postman.com/downloads/
- **Collection:** Import `Postman_Collection_Orders.json`
- **Best for:** Organized testing, automation
- **Pros:** Save requests, environments, collections
- **Cons:** Requires installation

### 3. cURL
- **Built-in:** Command line
- **Best for:** Quick command-line testing
- **Pros:** No GUI needed
- **Cons:** Verbose syntax

### 4. REST Client (VS Code Extension)
- **Extension:** REST Client by Huachao Mao
- **Best for:** Testing from within VS Code
- **Pros:** Keep tests with code
- **Cons:** VS Code specific

---

## ?? Environment Setup

### 1. Start API
```bash
cd API
dotnet run
```

### 2. Note the Port
```
Now listening on: https://localhost:7123
```

### 3. Open Swagger
```
https://localhost:7123
```

### 4. Prepare Tokens

**Admin Token:**
```bash
POST /api/auth/login
{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**Customer Token:**
```bash
POST /api/auth/login
{
  "email": "john.doe@example.com",
  "password": "User@123"
}
```

---

## ?? Authentication Testing

### Test 1: Register New User
```http
POST https://localhost:7123/api/auth/register
Content-Type: application/json

{
  "fullName": "Test User",
  "email": "testuser@example.com",
  "password": "Test@123",
  "confirmPassword": "Test@123"
}
```

**Expected:** 200 OK v?i user data và token

**Verify:**
- ? User created in database
- ? Password hashed (BCrypt)
- ? JWT token returned
- ? Token contains correct claims

### Test 2: Login v?i Valid Credentials
```http
POST https://localhost:7123/api/auth/login
Content-Type: application/json

{
  "email": "admin@electro.com",
  "password": "Admin@123"
}
```

**Expected:** 200 OK v?i token

**Verify:**
- ? Token generated
- ? ExpiresAt date is future
- ? User role included
- ? UserId included

### Test 3: Login v?i Invalid Credentials
```http
POST https://localhost:7123/api/auth/login
Content-Type: application/json

{
  "email": "admin@electro.com",
  "password": "WrongPassword"
}
```

**Expected:** 400 Bad Request

**Verify:**
- ? Error message returned
- ? No token generated
- ? Success = false

### Test 4: Validate Token
```http
POST https://localhost:7123/api/auth/validate
Content-Type: application/json

"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Expected:** 200 OK n?u valid

### Test 5: Get Current User
```http
GET https://localhost:7123/api/auth/me
Authorization: Bearer {token}
```

**Expected:** 200 OK v?i user info

**Verify:**
- ? Correct user data
- ? Role included
- ? No password returned

---

## ?? Product Testing

### Test 1: Get All Products
```http
GET https://localhost:7123/api/products?page=1&pageSize=10
```

**Expected:** 200 OK v?i paginated data

**Verify:**
- ? Items array present
- ? Pagination info correct
- ? Category name included
- ? HasPrevious = false (page 1)
- ? HasNext based on total

### Test 2: Get Product by ID
```http
GET https://localhost:7123/api/products/1
```

**Expected:** 200 OK v?i product details

**Verify:**
- ? All fields present
- ? Category info included
- ? Price formatted correctly

### Test 3: Search Products
```http
GET https://localhost:7123/api/products?keyword=dell
```

**Expected:** 200 OK v?i filtered results

**Verify:**
- ? Only matching products
- ? Case-insensitive search

### Test 4: Filter by Category
```http
GET https://localhost:7123/api/products?categoryId=1
```

**Expected:** 200 OK v?i products from category

### Test 5: Filter by Price Range
```http
GET https://localhost:7123/api/products?minPrice=500&maxPrice=1500
```

**Expected:** 200 OK v?i products in range

### Test 6: Sort Products
```http
GET https://localhost:7123/api/products?sortBy=price_asc
```

**Expected:** 200 OK, products sorted by price ascending

### Test 7: Create Product (Admin)
```http
POST https://localhost:7123/api/products
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Test Product",
  "price": 999.99,
  "description": "Test description",
  "stock": 100,
  "categoryId": 1
}
```

**Expected:** 200 OK v?i created product

**Verify:**
- ? Product created in DB
- ? ID assigned
- ? CreatedAt set

### Test 8: Create Product without Auth
```http
POST https://localhost:7123/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "price": 999.99
}
```

**Expected:** 401 Unauthorized

### Test 9: Create Product as Customer
```http
POST https://localhost:7123/api/products
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "name": "Test Product",
  "price": 999.99
}
```

**Expected:** 403 Forbidden

### Test 10: Update Product
```http
PUT https://localhost:7123/api/products/1
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Updated Product Name",
  "price": 1499.99,
  "description": "Updated description",
  "stock": 50,
  "categoryId": 1
}
```

**Expected:** 200 OK v?i updated product

**Verify:**
- ? Changes saved
- ? UpdatedAt changed

### Test 11: Delete Product
```http
DELETE https://localhost:7123/api/products/1
Authorization: Bearer {admin_token}
```

**Expected:** 200 OK

**Verify:**
- ? Product deleted
- ? Can't get product anymore

---

## ?? Category Testing

### Test 1: Get All Categories
```http
GET https://localhost:7123/api/categories
```

**Expected:** 200 OK v?i all categories

**Verify:**
- ? All 6 categories returned
- ? Product count included

### Test 2: Create Category (Admin)
```http
POST https://localhost:7123/api/categories
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Test Category",
  "description": "Test description"
}
```

**Expected:** 200 OK

**Verify:**
- ? Category created
- ? Unique name enforced

### Test 3: Duplicate Category Name
```http
POST https://localhost:7123/api/categories
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Laptops",
  "description": "Duplicate"
}
```

**Expected:** 400 Bad Request

---

## ?? Order Testing

### Test 1: Create Order (Customer)
```http
POST https://localhost:7123/api/orders
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "shippingAddress": "123 Test Street, District 1, HCMC",
  "shippingNote": "Call before delivery",
  "paymentMethod": "COD - Thanh toán khi nh?n hàng",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 5,
      "quantity": 1
    }
  ]
}
```

**Expected:** 200 OK v?i order created

**Verify:**
- ? Order code generated (ORD100021)
- ? Status = Pending (0)
- ? Total calculated correctly
- ? Stock decreased for products
- ? Order items created

### Test 2: Create Order v?i Invalid Product
```http
POST https://localhost:7123/api/orders
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "shippingAddress": "Test",
  "paymentMethod": "COD",
"items": [
    {
      "productId": 9999,
      "quantity": 1
    }
  ]
}
```

**Expected:** 400 Bad Request

**Error:** "S?n ph?m v?i ID 9999 không t?n t?i"

### Test 3: Create Order v?i Insufficient Stock
```http
POST https://localhost:7123/api/orders
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "shippingAddress": "Test",
  "paymentMethod": "COD",
  "items": [
    {
      "productId": 1,
 "quantity": 10000
    }
  ]
}
```

**Expected:** 400 Bad Request

**Error:** "S?n ph?m không ?? s? l??ng trong kho"

### Test 4: Get My Orders
```http
GET https://localhost:7123/api/orders/my-orders
Authorization: Bearer {customer_token}
```

**Expected:** 200 OK v?i user's orders

**Verify:**
- ? Only user's orders returned
- ? Orders sorted by date (newest first)
- ? Order items included

### Test 5: Get All Orders (Admin)
```http
GET https://localhost:7123/api/orders?page=1&pageSize=10
Authorization: Bearer {admin_token}
```

**Expected:** 200 OK v?i paginated orders

**Verify:**
- ? All users' orders visible
- ? Pagination working
- ? Customer names included

### Test 6: Filter Orders by Status
```http
GET https://localhost:7123/api/orders?status=0
Authorization: Bearer {admin_token}
```

**Expected:** 200 OK v?i pending orders only

### Test 7: Get Order by ID (Owner)
```http
GET https://localhost:7123/api/orders/1
Authorization: Bearer {customer_token}
```

**Expected:** 200 OK if user owns the order

### Test 8: Get Order by ID (Not Owner)
```http
GET https://localhost:7123/api/orders/1
Authorization: Bearer {different_customer_token}
```

**Expected:** 400 Bad Request

**Error:** "B?n không có quy?n xem ??n hàng này"

### Test 9: Update Order Status - Confirm
```http
PUT https://localhost:7123/api/orders/1/status
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "status": 1,
  "note": "Order confirmed"
}
```

**Expected:** 200 OK

**Verify:**
- ? Status changed to Confirmed
- ? UpdatedAt changed

### Test 10: Invalid Status Transition
```http
PUT https://localhost:7123/api/orders/1/status
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "status": 3
}
```

**Expected:** 400 Bad Request (if current status is Pending)

**Error:** "Không th? chuy?n tr?ng thái t? Ch? xác nh?n sang Hoàn thành"

### Test 11: Cancel Order (Customer)
```http
POST https://localhost:7123/api/orders/1/cancel
Authorization: Bearer {customer_token}
```

**Expected:** 200 OK if order is Pending

**Verify:**
- ? Status changed to Cancelled
- ? Stock restored for all items

### Test 12: Cancel Non-Pending Order
```http
POST https://localhost:7123/api/orders/2/cancel
Authorization: Bearer {customer_token}
```

**Expected:** 400 Bad Request (if order is not Pending)

**Error:** "Ch? có th? h?y ??n hàng ?ang ch? xác nh?n"

### Test 13: Get Order Statistics
```http
GET https://localhost:7123/api/orders/statistics
Authorization: Bearer {admin_token}
```

**Expected:** 200 OK v?i statistics

**Verify:**
- ? Total orders count
- ? Orders by status
- ? Total revenue (Completed only)
- ? Average order value

### Test 14: Statistics v?i Date Range
```http
GET https://localhost:7123/api/orders/statistics?fromDate=2024-02-01&toDate=2024-02-29
Authorization: Bearer {admin_token}
```

**Expected:** 200 OK v?i filtered statistics

---

## ? Error Handling Testing

### Test 1: Invalid JWT Token
```http
GET https://localhost:7123/api/products
Authorization: Bearer invalid_token_here
```

**Expected:** 401 Unauthorized

### Test 2: Expired Token
```http
GET https://localhost:7123/api/orders/my-orders
Authorization: Bearer {expired_token}
```

**Expected:** 401 Unauthorized

### Test 3: Missing Required Fields
```http
POST https://localhost:7123/api/products
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": ""
}
```

**Expected:** 400 Bad Request v?i validation errors

### Test 4: Invalid Data Types
```http
POST https://localhost:7123/api/products
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Product",
  "price": "not_a_number"
}
```

**Expected:** 400 Bad Request

### Test 5: Not Found Resource
```http
GET https://localhost:7123/api/products/99999
```

**Expected:** 404 Not Found

### Test 6: Unauthorized Access
```http
POST https://localhost:7123/api/products
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "name": "Product",
  "price": 100
}
```

**Expected:** 403 Forbidden

---

## ?? Complete Order Flow Testing

### Scenario: Hoàn thành ??n hàng t? ??u ??n cu?i

```bash
# Step 1: Customer Register
POST /api/auth/register
{
  "fullName": "New Customer",
  "email": "newcustomer@test.com",
  "password": "Test@123",
  "confirmPassword": "Test@123"
}
? Response: User created v?i token

# Step 2: Customer Browse Products
GET /api/products
? Response: List of 27 products

# Step 3: Customer View Product Details
GET /api/products/1
? Response: Dell XPS 13 details

# Step 4: Customer Create Order
POST /api/orders
Authorization: Bearer {customer_token}
{
  "shippingAddress": "123 Test St, District 1",
  "paymentMethod": "COD",
  "items": [
    { "productId": 1, "quantity": 1 }
  ]
}
? Response: Order created, OrderCode: ORD100021, Status: Pending
? Verify: Product stock decreased by 1

# Step 5: Customer View Order
GET /api/orders/my-orders
Authorization: Bearer {customer_token}
? Response: See new order in list

# Step 6: Admin Login
POST /api/auth/login
{ "email": "admin@electro.com", "password": "Admin@123" }
? Response: Admin token

# Step 7: Admin View All Pending Orders
GET /api/orders?status=0
Authorization: Bearer {admin_token}
? Response: List of pending orders including new order

# Step 8: Admin Confirm Order
PUT /api/orders/21/status
Authorization: Bearer {admin_token}
{ "status": 1, "note": "Confirmed" }
? Response: Order updated, Status: Confirmed

# Step 9: Admin Ship Order
PUT /api/orders/21/status
Authorization: Bearer {admin_token}
{ "status": 2, "note": "Shipping" }
? Response: Order updated, Status: Shipping

# Step 10: Admin Complete Order
PUT /api/orders/21/status
Authorization: Bearer {admin_token}
{ "status": 3, "note": "Delivered" }
? Response: Order updated, Status: Completed

# Step 11: Try to Update Completed Order
PUT /api/orders/21/status
Authorization: Bearer {admin_token}
{ "status": 2 }
? Response: Error - Cannot change completed order

# Step 12: Admin View Statistics
GET /api/orders/statistics
Authorization: Bearer {admin_token}
? Response: Updated statistics with new order in revenue
```

### Scenario: Customer h?y ??n hàng

```bash
# Step 1: Create Order
POST /api/orders
Authorization: Bearer {customer_token}
{
"shippingAddress": "Test",
  "paymentMethod": "COD",
  "items": [{ "productId": 1, "quantity": 2 }]
}
? Response: Order created (ORD100022)
? Verify: Stock decreased by 2

# Step 2: Get Product Stock
GET /api/products/1
? Note current stock value

# Step 3: Cancel Order
POST /api/orders/22/cancel
Authorization: Bearer {customer_token}
? Response: Order cancelled

# Step 4: Verify Stock Restored
GET /api/products/1
? Verify: Stock = previous + 2

# Step 5: Try to Cancel Again
POST /api/orders/22/cancel
Authorization: Bearer {customer_token}
? Response: Error - Order already cancelled
```

---

## ? Performance Testing

### Test 1: Large Page Size
```http
GET https://localhost:7123/api/products?pageSize=100
```

**Measure:** Response time

**Expected:** < 1 second

### Test 2: Complex Filter
```http
GET https://localhost:7123/api/products?categoryId=1&minPrice=500&maxPrice=2000&sortBy=price_asc&keyword=laptop
```

**Measure:** Response time

**Expected:** < 500ms

### Test 3: Concurrent Requests
```bash
# Use tool like Apache Bench or k6
ab -n 100 -c 10 https://localhost:7123/api/products
```

**Measure:** Requests per second

### Test 4: Database Query Performance
```csharp
// Enable SQL logging in appsettings.Development.json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

**Check:** Number of database queries per request

**Goal:** Minimize N+1 queries

---

## ?? Test Results Template

### Test Report

| Test Case | Expected | Actual | Status | Notes |
|-----------|----------|--------|--------|-------|
| Login Admin | 200 OK | 200 OK | ? Pass | Token valid |
| Create Order | 200 OK | 200 OK | ? Pass | Stock updated |
| Invalid Auth | 401 | 401 | ? Pass | Error message clear |
| ... | ... | ... | ... | ... |

### Performance Report

| Endpoint | Avg Response Time | Requests/sec | Status |
|----------|------------------|--------------|---------|
| GET /products | 45ms | 200 | ? Good |
| POST /orders | 120ms | 80 | ? Good |
| GET /orders | 65ms | 150 | ? Good |

---

## ?? Debugging Tests

### Enable Verbose Logging
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### View Logs
```bash
# Console logs
dotnet run

# Or in Visual Studio
View ? Output ? Show output from: Debug
```

### SQL Query Logging
```csharp
// In AppDbContext
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.LogTo(Console.WriteLine, new[] 
    { 
        DbLoggerCategory.Database.Command.Name 
    }, LogLevel.Information);
}
```

---

## ?? Test Checklist

### Before Release

#### Authentication
- [ ] Register v?i valid data works
- [ ] Register v?i duplicate email fails
- [ ] Login v?i valid credentials works
- [ ] Login v?i invalid credentials fails
- [ ] Token validation works
- [ ] Expired token rejected
- [ ] Get current user works

#### Products
- [ ] Get all products v?i pagination works
- [ ] Get product by ID works
- [ ] Search products works
- [ ] Filter by category works
- [ ] Filter by price works
- [ ] Sort products works
- [ ] Create product (Admin) works
- [ ] Create product (Customer) forbidden
- [ ] Update product works
- [ ] Delete product works

#### Categories
- [ ] Get all categories works
- [ ] Get category by ID works
- [ ] Create category works
- [ ] Duplicate category name fails
- [ ] Update category works
- [ ] Delete category works

#### Orders
- [ ] Create order works
- [ ] Stock decreases on create
- [ ] Invalid product fails
- [ ] Insufficient stock fails
- [ ] Get my orders works
- [ ] Get all orders (Admin) works
- [ ] Customer can't see others' orders
- [ ] Update status works (Admin)
- [ ] Invalid transition fails
- [ ] Cancel order works
- [ ] Stock restores on cancel
- [ ] Can't cancel non-pending order
- [ ] Statistics calculation correct

#### Authorization
- [ ] Endpoints require auth
- [ ] Role checks work
- [ ] Ownership checks work
- [ ] 401 for missing token
- [ ] 403 for insufficient permissions

#### Error Handling
- [ ] Validation errors return 400
- [ ] Not found returns 404
- [ ] Unauthorized returns 401
- [ ] Forbidden returns 403
- [ ] Server errors return 500
- [ ] Error messages are clear

---

## ?? Test Coverage Goals

### Current Status
- Manual tests: ? 100%
- Unit tests: ? 0% (Planned)
- Integration tests: ? 0% (Planned)

### Goals
- Unit tests: 80%+
- Integration tests: 60%+
- E2E tests: 40%+

---

## ?? Testing Resources

### Tools
- [Postman](https://www.postman.com/)
- [xUnit](https://xunit.net/)
- [Moq](https://github.com/moq/moq4)
- [FluentAssertions](https://fluentassertions.com/)

### Learning
- [ASP.NET Core Testing](https://docs.microsoft.com/aspnet/core/test/)
- [Unit Testing Best Practices](https://docs.microsoft.com/dotnet/core/testing/unit-testing-best-practices)
- [Integration Testing](https://docs.microsoft.com/aspnet/core/test/integration-tests)

---

## ? Quick Test Commands

```bash
# Test all endpoints
dotnet test

# Test specific class
dotnet test --filter ProductServiceTests

# Test with coverage
dotnet test /p:CollectCoverage=true

# Test with detailed output
dotnet test --verbosity detailed
```

---

<div align="center">

**Happy Testing! ??**

Comprehensive testing ensures quality code.

[Back to README](./README.md)

</div>
