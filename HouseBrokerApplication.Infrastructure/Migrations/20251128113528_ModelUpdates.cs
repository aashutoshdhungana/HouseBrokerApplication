using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseBrokerApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferId1",
                table: "Deals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_OfferId1",
                table: "Deals",
                column: "OfferId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Offers_OfferId1",
                table: "Deals",
                column: "OfferId1",
                principalTable: "Offers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Offers_OfferId1",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_OfferId1",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OfferId1",
                table: "Deals");
        }
    }
}
