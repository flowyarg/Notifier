using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifier.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Subscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriberChatId = table.Column<long>(type: "bigint", nullable: false),
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    PlaylistOwnerId = table.Column<int>(type: "integer", nullable: false),
                    LastSyncDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.SubscriberChatId, x.PlaylistOwnerId, x.PlaylistId });
                    table.ForeignKey(
                        name: "FK_Subscriptions_Playlists_PlaylistId_PlaylistOwnerId",
                        columns: x => new { x.PlaylistId, x.PlaylistOwnerId },
                        principalTable: "Playlists",
                        principalColumns: new[] { "Id", "OwnerId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlaylistId_PlaylistOwnerId",
                table: "Subscriptions",
                columns: new[] { "PlaylistId", "PlaylistOwnerId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}
