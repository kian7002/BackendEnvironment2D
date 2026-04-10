using Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendEnvironment2d.Models;

public class Object2D
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EnvironmentId { get; set; }

    [ForeignKey(nameof(EnvironmentId))]
    public Environment2D? Environment { get; set; }

    [Required]
    public string PrefabId { get; set; } = string.Empty;

    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float RotationZ { get; set; }
    public int SortingLayer { get; set; }
}