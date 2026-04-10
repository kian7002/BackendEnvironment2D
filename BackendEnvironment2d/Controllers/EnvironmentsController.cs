using System.Security.Claims;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("environments")]

public class EnvironmentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public EnvironmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var environments = await _db.Environments
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.SlotIndex)
            .ToListAsync();

        return Ok(environments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Environment2D environment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (string.IsNullOrWhiteSpace(environment.Name) || environment.Name.Length > 25)
            return BadRequest("Naam moet tussen 1 en 25 karakters zijn.");

        if (environment.SlotIndex < 0 || environment.SlotIndex > 4)
            return BadRequest("Ongeldig slot index.");

        var count = await _db.Environments.CountAsync(e => e.UserId == userId);
        if (count >= 5)
            return BadRequest("Je mag maximaal 5 werelden hebben.");

        // ⭐ Check of slot al bestaat
        var slotExists = await _db.Environments.AnyAsync(e =>
            e.UserId == userId &&
            e.SlotIndex == environment.SlotIndex);

        if (slotExists)
            return BadRequest("Er bestaat al een wereld op dit slot.");

        // Check duplicate naam
        var duplicate = await _db.Environments.AnyAsync(e =>
            e.UserId == userId &&
            e.Name == environment.Name);

        if (duplicate)
            return BadRequest("Je hebt al een wereld met deze naam.");

        environment.Id = Guid.NewGuid();
        environment.UserId = userId;

        _db.Environments.Add(environment);
        await _db.SaveChangesAsync();

        return Ok(environment);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Environment2D updatedEnvironment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var environment = await _db.Environments
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (environment == null)
            return NotFound("Wereld niet gevonden.");

        if (string.IsNullOrWhiteSpace(updatedEnvironment.Name) || updatedEnvironment.Name.Length > 25)
            return BadRequest("Naam moet tussen 1 en 25 karakters zijn.");

        var duplicate = await _db.Environments.AnyAsync(e =>
            e.UserId == userId &&
            e.Id != id &&
            e.Name == updatedEnvironment.Name);

        if (duplicate)
            return BadRequest("Je hebt al een wereld met deze naam.");

        environment.Name = updatedEnvironment.Name;
        environment.MaxLength = updatedEnvironment.MaxLength;
        environment.MaxHeight = updatedEnvironment.MaxHeight;

        // BELANGRIJK: SlotIndex NIET aanpassen bij rename/update
        // environment.SlotIndex blijft zoals hij al in de database staat

        await _db.SaveChangesAsync();

        return Ok(environment);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var environment = await _db.Environments
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (environment == null)
            return NotFound("Wereld niet gevonden.");

        _db.Environments.Remove(environment);
        await _db.SaveChangesAsync();

        return Ok("Environment deleted");
    }
}