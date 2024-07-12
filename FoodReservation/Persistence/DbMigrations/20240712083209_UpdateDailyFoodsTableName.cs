using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodReservation.Infrastructure.Persistence.DbMigrations
{
    /// <inheritdoc />
    public partial class UpdateDailyFoodsTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_FoodMetadata_FoodId_Date",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "FoodMetadata");

            migrationBuilder.CreateTable(
                name: "DailyFoods",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyFoods", x => new { x.FoodId, x.Date });
                    table.ForeignKey(
                        name: "FK_DailyFoods_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_DailyFoods_FoodId_Date",
                table: "Reservations",
                columns: new[] { "FoodId", "Date" },
                principalTable: "DailyFoods",
                principalColumns: new[] { "FoodId", "Date" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_DailyFoods_FoodId_Date",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "DailyFoods");

            migrationBuilder.CreateTable(
                name: "FoodMetadata",
                columns: table => new
                {
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodMetadata", x => new { x.FoodId, x.Date });
                    table.ForeignKey(
                        name: "FK_FoodMetadata_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_FoodMetadata_FoodId_Date",
                table: "Reservations",
                columns: new[] { "FoodId", "Date" },
                principalTable: "FoodMetadata",
                principalColumns: new[] { "FoodId", "Date" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
