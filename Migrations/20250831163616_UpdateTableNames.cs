using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace IchigoHoshimiya.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiringEpisodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AiringAnime",
                table: "AiringAnime");

            migrationBuilder.RenameTable(
                name: "AiringAnime",
                newName: "airing_anime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_airing_anime",
                table: "airing_anime",
                column: "id");

            migrationBuilder.CreateTable(
                name: "airing_episode",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    episode_number = table.Column<int>(type: "int", nullable: false),
                    airing_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    anime_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airing_episode", x => x.id);
                    table.ForeignKey(
                        name: "FK_airing_episode_airing_anime_anime_id",
                        column: x => x.anime_id,
                        principalTable: "airing_anime",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_airing_episode_anime_id",
                table: "airing_episode",
                column: "anime_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "airing_episode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_airing_anime",
                table: "airing_anime");

            migrationBuilder.RenameTable(
                name: "airing_anime",
                newName: "AiringAnime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiringAnime",
                table: "AiringAnime",
                column: "id");

            migrationBuilder.CreateTable(
                name: "AiringEpisodes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    anime_id = table.Column<long>(type: "bigint", nullable: false),
                    airing_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    episode_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiringEpisodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_AiringEpisodes_AiringAnime_anime_id",
                        column: x => x.anime_id,
                        principalTable: "AiringAnime",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AiringEpisodes_anime_id",
                table: "AiringEpisodes",
                column: "anime_id");
        }
    }
}
