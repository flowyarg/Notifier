using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifier.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UrlsforPlaylistsandOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Playlists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Owners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Owners");
        }
    }
}
