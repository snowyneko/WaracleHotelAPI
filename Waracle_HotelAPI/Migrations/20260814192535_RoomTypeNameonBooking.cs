using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waracle_HotelAPI.Migrations
{
    /// <inheritdoc />
    public partial class RoomTypeNameonBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Bookings");
        }
    }
}
