using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
     name: "CreatedAt",
        table: "RefreshTokens",
     type: "datetime2",
    nullable: false,
     defaultValue: DateTime.UtcNow);
 }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
  migrationBuilder.DropColumn(
       name: "CreatedAt",
  table: "RefreshTokens");
        }
    }
}
