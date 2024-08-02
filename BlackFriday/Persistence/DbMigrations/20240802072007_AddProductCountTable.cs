using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackFriday.Infrastructure.Persistence.DbMigrations
{
    /// <inheritdoc />
    public partial class AddProductCountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCounts",
                columns: table => new
                {
                    Asin = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCounts", x => x.Asin);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCounts");
        }
    }
}
