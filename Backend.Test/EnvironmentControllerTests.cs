using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

public class EnvironmentsControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private ClaimsPrincipal GetUser()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
        }, "TestAuth"));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
    {
        var db = GetDbContext();

        var controller = new EnvironmentsController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = GetUser()
            }
        };

        var environment = new Environment2D
        {
            Name = "",
            SlotIndex = 0,
            MaxHeight = 20,
            MaxLength = 50
        };

        var result = await controller.Create(environment);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenEnvironmentIsValid()
    {
        var db = GetDbContext();

        var controller = new EnvironmentsController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = GetUser()
            }
        };

        var environment = new Environment2D
        {
            Name = "TestWorld",
            SlotIndex = 0,
            MaxHeight = 20,
            MaxLength = 50
        };

        var result = await controller.Create(environment);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDuplicateSlot()
    {
        var db = GetDbContext();

        db.Environments.Add(new Environment2D
        {
            Id = Guid.NewGuid(),
            Name = "World1",
            SlotIndex = 0,
            UserId = "test-user-id",
            MaxHeight = 20,
            MaxLength = 50
        });

        await db.SaveChangesAsync();

        var controller = new EnvironmentsController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = GetUser()
            }
        };

        var environment = new Environment2D
        {
            Name = "World2",
            SlotIndex = 0,
            MaxHeight = 20,
            MaxLength = 50
        };

        var result = await controller.Create(environment);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
