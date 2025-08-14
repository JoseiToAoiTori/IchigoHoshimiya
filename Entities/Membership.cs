using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("memberships")]
[Index("ArtistId", Name = "memberships_artist_id_foreign")]
[Index("MemberId", Name = "memberships_member_id_foreign")]
public partial class Membership
{
    [Key]
    [Column("membership_id")]
    public ulong MembershipId { get; set; }

    [Column("artist_id")]
    public ulong ArtistId { get; set; }

    [Column("member_id")]
    public ulong MemberId { get; set; }

    [Column("alias")]
    [StringLength(255)]
    public string? Alias { get; set; }

    [Column("as")]
    [StringLength(255)]
    public string? As { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("MembershipArtists")]
    public virtual Artist Artist { get; set; } = null!;

    [ForeignKey("MemberId")]
    [InverseProperty("MembershipMembers")]
    public virtual Artist Member { get; set; } = null!;
}
