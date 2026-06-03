using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixQuantitySpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quanbtity",
                table: "CartItems",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "CartItems",
                newName: "Quanbtity");
        }
    }
}
