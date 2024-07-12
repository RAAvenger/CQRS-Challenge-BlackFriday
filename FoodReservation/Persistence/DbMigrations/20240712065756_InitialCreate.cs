using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodReservation.Infrastructure.Persistence.DbMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodMetadata",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => new { x.FoodId, x.UserId, x.Date });
                    table.ForeignKey(
                        name: "FK_Reservations_FoodMetadata_FoodId_Date",
                        columns: x => new { x.FoodId, x.Date },
                        principalTable: "FoodMetadata",
                        principalColumns: new[] { "FoodId", "Date" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_FoodId_Date",
                table: "Reservations",
                columns: new[] { "FoodId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "FoodMetadata");

            migrationBuilder.DropTable(
                name: "Foods");
        }
    }
}
