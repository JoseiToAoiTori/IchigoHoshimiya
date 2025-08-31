using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("StudioId", "ResourceId")]
[Table("studio_resource")]
[Index("ResourceId", Name = "studio_resource_resource_id_foreign")]
public class StudioResource
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("studio_id")] public ulong StudioId { get; set; }

    [Key] [Column("resource_id")] public ulong ResourceId { get; set; }

    [Column("as")] [StringLength(255)] public string? As { get; set; }

    [ForeignKey("ResourceId")]
    [InverseProperty("StudioResources")]
    public virtual Resource Resource { get; set; } = null!;

    [ForeignKey("StudioId")]
    [InverseProperty("StudioResources")]
    public virtual Studio Studio { get; set; } = null!;
}