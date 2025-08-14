using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("artists")]
public partial class Artist
{
    [Key]
    [Column("artist_id")]
    public ulong ArtistId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("information", TypeName = "text")]
    public string? Information { get; set; }

    [InverseProperty("Artist")]
    public virtual ICollection<ArtistImage> ArtistImages { get; set; } = new List<ArtistImage>();

    [InverseProperty("Artist")]
    public virtual ICollection<ArtistMember> ArtistMemberArtists { get; set; } = new List<ArtistMember>();

    [InverseProperty("Member")]
    public virtual ICollection<ArtistMember> ArtistMemberMembers { get; set; } = new List<ArtistMember>();

    [InverseProperty("Artist")]
    public virtual ICollection<ArtistResource> ArtistResources { get; set; } = new List<ArtistResource>();

    [InverseProperty("Artist")]
    public virtual ICollection<ArtistSong> ArtistSongs { get; set; } = new List<ArtistSong>();

    [InverseProperty("Artist")]
    public virtual ICollection<Membership> MembershipArtists { get; set; } = new List<Membership>();

    [InverseProperty("Member")]
    public virtual ICollection<Membership> MembershipMembers { get; set; } = new List<Membership>();
}
