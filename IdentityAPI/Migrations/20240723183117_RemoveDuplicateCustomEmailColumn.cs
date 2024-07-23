using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateCustomEmailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
