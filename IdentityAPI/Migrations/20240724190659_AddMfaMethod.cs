using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MfaMethod",
                table: "User",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MfaMethod",
                table: "User");
        }
    }
}
