using BackendEnvironment2d.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Models;

public class Environment2D
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    [Range(20, 200)]
    public int MaxLength { get; set; }

    [Range(10, 100)]
    public int MaxHeight { get; set; }

  
    public string? UserId { get; set; } = null;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    public List<Object2D> Objects { get; set; } = new();

    public int SlotIndex { get; set; }
}