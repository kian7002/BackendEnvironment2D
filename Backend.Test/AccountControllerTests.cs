using Backend.Controllers;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class AccountControllerTests
{
    private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private IConfiguration GetConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "SuperSecretKey12345678901234567890" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
    {
        var userManagerMock = GetUserManagerMock();
        var config = GetConfiguration();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(new ApplicationUser { Email = "test@test.com" });

        var controller = new AccountController(userManagerMock.Object, config);

        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var result = await controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUserIsCreated()
    {
        var userManagerMock = GetUserManagerMock();
        var config = GetConfiguration();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("new@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = new AccountController(userManagerMock.Object, config);

        var dto = new RegisterDto
        {
            Email = "new@test.com",
            Password = "Test123!"
        };

        var result = await controller.Register(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Register success", okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
    {
        var userManagerMock = GetUserManagerMock();
        var config = GetConfiguration();

        userManagerMock
            .Setup(x => x.FindByEmailAsync("missing@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var controller = new AccountController(userManagerMock.Object, config);

        var dto = new LoginDto
        {
            Email = "missing@test.com",
            Password = "Test123!"
        };

        var result = await controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
    {
        var userManagerMock = GetUserManagerMock();
        var config = GetConfiguration();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@test.com",
            UserName = "test@test.com"
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "Test123!"))
            .ReturnsAsync(true);

        var controller = new AccountController(userManagerMock.Object, config);

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var result = await controller.Login(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenResponse = Assert.IsType<TokenResponseDto>(okResult.Value);

        Assert.Equal("Bearer", tokenResponse.TokenType);
        Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));
    }
}