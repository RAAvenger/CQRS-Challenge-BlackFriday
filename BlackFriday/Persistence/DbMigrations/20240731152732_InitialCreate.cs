using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackFriday.Infrastructure.Persistence.DbMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    asin = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    boughtInLastMonth = table.Column<long>(type: "bigint", nullable: false),
                    categoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    imgUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    isBestSeller = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<decimal>(type: "numeric(28,6)", precision: 28, scale: 6, nullable: false),
                    productUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reviews = table.Column<long>(type: "bigint", nullable: false),
                    stars = table.Column<decimal>(type: "numeric(28,6)", precision: 28, scale: 6, nullable: false),
                    title = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.asin);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
