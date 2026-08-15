using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waracle_HotelAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedBookingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId_DepartureDate_ArrivalDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DepartureDate_ArrivalDate",
                table: "Bookings",
                columns: new[] { "DepartureDate", "ArrivalDate" })
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_DepartureDate_ArrivalDate",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId_DepartureDate_ArrivalDate",
                table: "Bookings",
                columns: new[] { "RoomId", "DepartureDate", "ArrivalDate" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
