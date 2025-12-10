using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("artists_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    album = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    spotify_id = table.Column<string>(type: "text", nullable: false),
                    spotify_uri = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("songs_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    profile_image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "song_artists",
                columns: table => new
                {
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_song_artists", x => new { x.song_id, x.artist_id });
                    table.ForeignKey(
                        name: "fk_song_artists_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_song_artists_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    host_id = table.Column<int>(type: "integer", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("playlists_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_users_host_id",
                        column: x => x.host_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "playlist_collaborators",
                columns: table => new
                {
                    playlist_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_collaborators", x => new { x.playlist_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_playlist_collaborators_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_collaborators_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_songs",
                columns: table => new
                {
                    playlist_id = table.Column<int>(type: "integer", nullable: false),
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: true),
                    added_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("playlist_songs_pkey", x => new { x.playlist_id, x.song_id });
                    table.ForeignKey(
                        name: "fk_playlist_songs_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_songs_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_songs_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_playlist_collaborators_user_id",
                table: "playlist_collaborators",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_songs_added_by_user_id",
                table: "playlist_songs",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlist_songs_song_id",
                table: "playlist_songs",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_host_id",
                table: "playlists",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "ix_song_artists_artist_id",
                table: "song_artists",
                column: "artist_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "playlist_collaborators");

            migrationBuilder.DropTable(
                name: "playlist_songs");

            migrationBuilder.DropTable(
                name: "song_artists");

            migrationBuilder.DropTable(
                name: "playlists");

            migrationBuilder.DropTable(
                name: "artists");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
