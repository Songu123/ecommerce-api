# ?? Contributing to E-commerce API

First off, thank you for considering contributing to E-commerce API! It's people like you that make this project better.

## ?? Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Issue Guidelines](#issue-guidelines)

---

## ?? Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inspiring community for all.

### Our Standards

**Positive behaviors:**
- ? Using welcoming and inclusive language
- ? Being respectful of differing viewpoints
- ? Gracefully accepting constructive criticism
- ? Focusing on what is best for the community

**Unacceptable behaviors:**
- ? Harassment or discriminatory language
- ? Trolling or insulting comments
- ? Public or private harassment
- ? Publishing others' private information

---

## ?? How Can I Contribute?

### ?? Reporting Bugs

Before creating bug reports, please check existing issues. When you create a bug report, include as many details as possible:

**Template:**
```markdown
**Describe the bug**
A clear description of what the bug is.

**To Reproduce**
Steps to reproduce:
1. Go to '...'
2. Click on '...'
3. See error

**Expected behavior**
What you expected to happen.

**Screenshots**
If applicable, add screenshots.

**Environment:**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 8.0]
- Browser: [e.g., Chrome 120]

**Additional context**
Any other relevant information.
```

### ?? Suggesting Features

Feature requests are welcome! Before suggesting:

1. **Check** if feature already exists
2. **Search** existing feature requests
3. **Describe** the feature clearly
4. **Explain** why it would be useful

**Template:**
```markdown
**Is your feature related to a problem?**
A clear description of the problem.

**Describe the solution**
What you want to happen.

**Describe alternatives**
Other solutions you've considered.

**Additional context**
Mockups, examples, etc.
```

### ?? Improving Documentation

Documentation improvements are always welcome:

- Fix typos or grammar
- Add code examples
- Clarify existing documentation
- Add missing documentation
- Translate to other languages

### ?? Code Contributions

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

---

## ??? Development Setup

### Prerequisites

- .NET 8 SDK
- SQL Server (Local or Express)
- Visual Studio 2022 / VS Code
- Git

### Setup Steps

```bash
# 1. Fork and clone
git clone https://github.com/yourusername/ecommerce-api.git
cd ecommerce-api/API

# 2. Install dependencies
dotnet restore

# 3. Setup database
dotnet ef database update

# 4. Run application
dotnet run

# 5. Run tests (when available)
dotnet test
```

### Development Workflow

```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes
# ... code ...

# Run build
dotnet build

# Check for errors
dotnet build --no-incremental

# Run application
dotnet run

# Test your changes
# ... test ...

# Commit
git add .
git commit -m "feat(scope): description"

# Push
git push origin feature/your-feature-name

# Create Pull Request on GitHub
```

---

## ?? Coding Standards

### C# Conventions

#### Naming Conventions
```csharp
// Classes, Methods, Properties - PascalCase
public class ProductService { }
public void CreateProduct() { }
public string ProductName { get; set; }

// Private fields - _camelCase
private readonly IProductRepository _productRepo;

// Parameters, local variables - camelCase
public void Method(int productId)
{
    var product = ...;
}

// Constants - PascalCase
public const int MaxPageSize = 100;

// Interfaces - IPascalCase
public interface IProductService { }
```

#### Code Organization
```csharp
public class ExampleClass
{
    // 1. Constants
  private const int MaxRetries = 3;

    // 2. Fields
    private readonly IService _service;
    private readonly ILogger _logger;

    // 3. Constructor
    public ExampleClass(IService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
    }

    // 4. Public Properties
    public int Id { get; set; }

 // 5. Public Methods
public async Task<Result> MethodAsync()
    {
        // Implementation
    }

    // 6. Private Methods
    private void HelperMethod()
  {
        // Implementation
    }
}
```

### File Organization

```
Feature/
??? Models/Feature.cs
??? DTOs/
?   ??? Request/FeatureRequest.cs
?   ??? Response/FeatureResponse.cs
??? Repositories/
?   ??? Interfaces/IFeatureRepository.cs
?   ??? FeatureRepository.cs
??? Services/
?   ??? Interfaces/IFeatureService.cs
?   ??? FeatureService.cs
??? Controllers/FeatureController.cs
```

### Documentation Standards

#### XML Documentation
```csharp
/// <summary>
/// Brief description of the method
/// </summary>
/// <param name="id">Parameter description</param>
/// <returns>Return value description</returns>
/// <exception cref="NotFoundException">When entity not found</exception>
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    // Implementation
}
```

#### Comments
```csharp
// Use comments for complex logic only
// Avoid obvious comments

// ? Bad
var user = GetUser(); // Get user

// ? Good
// Calculate discount based on user tier and order history
var discount = CalculateDiscount(user, order);
```

### Code Quality

#### ? Good Practices
```csharp
// Use meaningful names
public async Task<Order> CreateOrderAsync(CreateOrderRequest request)

// Use async/await
await _repository.SaveAsync();

// Validate early
if (product == null)
    throw new NotFoundException("Product not found");

// Use LINQ
var activeUsers = users.Where(u => u.IsActive).ToList();

// Use dependency injection
public class Service
{
    private readonly IRepository _repo;
    public Service(IRepository repo) => _repo = repo;
}
```

#### ? Avoid
```csharp
// Don't use var everywhere
var x = GetSomething(); // What is x?

// Don't ignore exceptions
try { } catch { } // ?

// Don't use magic numbers
if (status == 3) { } // What is 3?

// Don't repeat code
// Use methods, avoid copy-paste

// Don't mix responsibilities
// Keep controllers thin, logic in services
```

---

## ?? Commit Guidelines

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat:** New feature
- **fix:** Bug fix
- **docs:** Documentation changes
- **style:** Code style changes (formatting, semicolons)
- **refactor:** Code refactoring
- **test:** Adding tests
- **chore:** Maintenance tasks

### Examples

```bash
# Feature
feat(orders): add order cancellation endpoint

# Bug fix
fix(auth): resolve token expiry calculation

# Documentation
docs(readme): update installation steps

# Refactor
refactor(services): simplify product service logic

# Multiple changes
feat(products): add image upload support

- Add image upload endpoint
- Implement file validation
- Add image optimization
- Update product model
```

### Scope

Use the feature name:
- auth
- products
- categories
- orders
- users
- database
- docs

### Rules

- Use present tense ("add" not "added")
- Use imperative mood ("move" not "moves")
- Don't capitalize first letter
- No period at the end
- Max 72 characters for subject
- Wrap body at 72 characters

---

## ?? Pull Request Process

### Before Submitting

1. ? **Update** your branch with latest main
2. ? **Build** succeeds without warnings
3. ? **Test** your changes thoroughly
4. ? **Update** documentation if needed
5. ? **Follow** coding standards
6. ? **Add** tests for new features
7. ? **Check** no merge conflicts

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings
- [ ] Tests pass

## Screenshots (if applicable)

## Related Issues
Fixes #123
```

### Review Process

1. **Automated Checks** run (when CI/CD configured)
2. **Code Review** by maintainer
3. **Feedback** addressed
4. **Approval** granted
5. **Merge** to main

### Merging

- **Squash commits** for clean history
- **Delete branch** after merge
- **Update changelog** if needed

---

## ?? Issue Guidelines

### Creating Issues

#### Bug Report
```markdown
**Bug Description**
Clear description of the bug

**Steps to Reproduce**
1. Step 1
2. Step 2
3. Step 3

**Expected Behavior**
What should happen

**Actual Behavior**
What actually happens

**Environment**
- OS: Windows 11
- .NET: 8.0
- SQL Server: 2022

**Logs/Screenshots**
Attach relevant logs or screenshots
```

#### Feature Request
```markdown
**Feature Description**
What feature you want

**Use Case**
Why this feature is needed

**Proposed Solution**
How you think it should work

**Alternatives**
Other options considered

**Priority**
Low / Medium / High
```

### Issue Labels

- `bug` - Something isn't working
- `enhancement` - New feature request
- `documentation` - Documentation improvements
- `good first issue` - Good for newcomers
- `help wanted` - Need community help
- `question` - Question about usage
- `wontfix` - Will not be fixed
- `duplicate` - Duplicate issue
- `invalid` - Invalid issue

---

## ?? Testing Guidelines

### Unit Tests

```csharp
[Fact]
public async Task CreateProduct_ValidRequest_ReturnsProduct()
{
    // Arrange
    var request = new CreateProductRequest { /* ... */ };
    var mockRepo = new Mock<IProductRepository>();
    var service = new ProductService(mockRepo.Object);

    // Act
    var result = await service.CreateProductAsync(request);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(request.Name, result.Name);
}
```

### Integration Tests

```csharp
[Fact]
public async Task Login_ValidCredentials_ReturnsToken()
{
    // Arrange
    var client = _factory.CreateClient();
 var request = new LoginRequest { /* ... */ };

    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login", request);

    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
    Assert.NotNull(result.Token);
}
```

---

## ?? Resources for Contributors

### Documentation
- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [C# Programming Guide](https://docs.microsoft.com/dotnet/csharp)

### Tools
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [VS Code](https://code.visualstudio.com/)
- [Postman](https://www.postman.com/)
- [SQL Server Management Studio](https://docs.microsoft.com/sql/ssms)

### Community
- [Stack Overflow - ASP.NET Core](https://stackoverflow.com/questions/tagged/asp.net-core)
- [Reddit - r/dotnet](https://reddit.com/r/dotnet)
- [Discord - .NET](https://discord.gg/dotnet)

---

## ? Questions?

### Where to Ask

- ?? **General Questions:** Use [Discussions](https://github.com/yourusername/ecommerce-api/discussions)
- ?? **Bug Reports:** Create an [Issue](https://github.com/yourusername/ecommerce-api/issues)
- ?? **Private Questions:** Email your.email@example.com

### Response Time

- Issues: Within 48 hours
- Pull Requests: Within 1 week
- Discussions: Within 24 hours

---

## ?? Recognition

Contributors will be:
- Listed in README.md
- Credited in CHANGELOG.md
- Thanked in release notes
- Given GitHub contributor badge

---

## ?? License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

## ?? Thank You!

Your contributions make this project better for everyone. Whether it's:

- ?? Bug reports
- ?? Feature suggestions
- ?? Documentation improvements
- ?? Code contributions
- ? Starring the repository
- ?? Sharing with others

**Every contribution matters!**

---

<div align="center">

**Happy Contributing! ??**

Made with ?? by the community

[Back to README](./README.md)

</div>
