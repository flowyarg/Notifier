using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notifier.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewMatrixAccessTokenProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Scopes",
                table: "MatrixAccessTokens",
                newName: "Scope");

            migrationBuilder.AddColumn<string>(
                name: "DeviceCode",
                table: "MatrixAccessTokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceCode",
                table: "MatrixAccessTokens");

            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "MatrixAccessTokens",
                newName: "Scopes");
        }
    }
}
