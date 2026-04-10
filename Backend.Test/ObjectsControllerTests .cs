using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using BackendEnvironment2d.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

public class ObjectsControllerTests
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

    private ObjectsController GetController(ApplicationDbContext db)
    {
        var controller = new ObjectsController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = GetUser()
            }
        };
        return controller;
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenEnvironmentDoesNotExist()
    {
        var db = GetDbContext();
        var controller = GetController(db);

        var result = await controller.GetAll(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenObjectIsValid()
    {
        var db = GetDbContext();

        var environment = new Environment2D
        {
            Id = Guid.NewGuid(),
            Name = "TestWorld",
            UserId = "test-user-id",
            SlotIndex = 0,
            MaxLength = 50,
            MaxHeight = 20
        };

        db.Environments.Add(environment);
        await db.SaveChangesAsync();

        var controller = GetController(db);

        var object2D = new Object2D
        {
            PrefabId = "tree_01",
            PositionX = 1,
            PositionY = 2,
            ScaleX = 1,
            ScaleY = 1,
            RotationZ = 0,
            SortingLayer = 1
        };

        var result = await controller.Create(environment.Id, object2D);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdObject = Assert.IsType<Object2D>(okResult.Value);

        Assert.Equal(environment.Id, createdObject.EnvironmentId);
        Assert.Equal("tree_01", createdObject.PrefabId);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenObjectDoesNotExist()
    {
        var db = GetDbContext();

        var environment = new Environment2D
        {
            Id = Guid.NewGuid(),
            Name = "TestWorld",
            UserId = "test-user-id",
            SlotIndex = 0,
            MaxLength = 50,
            MaxHeight = 20
        };

        db.Environments.Add(environment);
        await db.SaveChangesAsync();

        var controller = GetController(db);

        var updated = new Object2D
        {
            PrefabId = "rock_01",
            PositionX = 5,
            PositionY = 6,
            ScaleX = 2,
            ScaleY = 2,
            RotationZ = 10,
            SortingLayer = 2
        };

        var result = await controller.Update(environment.Id, Guid.NewGuid(), updated);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}