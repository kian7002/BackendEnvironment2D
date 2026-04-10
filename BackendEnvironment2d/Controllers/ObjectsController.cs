using Backend.Data;
using Backend.Models;
using BackendEnvironment2d.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("environments/{environmentId:guid}/objects")]
public class ObjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ObjectsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid environmentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var ownsEnvironment = await _db.Environments
            .AnyAsync(e => e.Id == environmentId && e.UserId == userId);

        if (!ownsEnvironment)
            return NotFound("Wereld niet gevonden.");

        var objects = await _db.Objects
            .Where(o => o.EnvironmentId == environmentId)
            .ToListAsync();

        return Ok(objects);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid environmentId, Object2D object2D)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var environment = await _db.Environments
            .FirstOrDefaultAsync(e => e.Id == environmentId && e.UserId == userId);

        if (environment == null)
            return NotFound("Wereld niet gevonden.");

        object2D.Id = Guid.NewGuid();
        object2D.EnvironmentId = environmentId;

        _db.Objects.Add(object2D);
        await _db.SaveChangesAsync();

        return Ok(object2D);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid environmentId, Guid id, Object2D updated)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var environment = await _db.Environments
            .FirstOrDefaultAsync(e => e.Id == environmentId && e.UserId == userId);

        if (environment == null)
            return NotFound("Wereld niet gevonden.");

        var object2D = await _db.Objects
            .FirstOrDefaultAsync(o => o.Id == id && o.EnvironmentId == environmentId);

        if (object2D == null)
            return NotFound("Object niet gevonden.");

        object2D.PrefabId = updated.PrefabId;
        object2D.PositionX = updated.PositionX;
        object2D.PositionY = updated.PositionY;
        object2D.ScaleX = updated.ScaleX;
        object2D.ScaleY = updated.ScaleY;
        object2D.RotationZ = updated.RotationZ;
        object2D.SortingLayer = updated.SortingLayer;

        await _db.SaveChangesAsync();
        return Ok("Object updated");
    }
}