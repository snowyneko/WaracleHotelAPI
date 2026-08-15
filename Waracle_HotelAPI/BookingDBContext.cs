using Microsoft.EntityFrameworkCore;
using Waracle_HotelAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Waracle_HotelAPI
{
    public class BookingDBContext : DbContext
    {
        string DBString;
        public BookingDBContext(IConfiguration config)
        {
            DBString = config.GetConnectionString("DefaultConnection") ??"";
        }
        public string DBConnectionString { get; set; } = "";

        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>(entity => {
                entity.HasKey(r => r.Id).IsClustered(false);
                entity.HasIndex(r => new { r.HotelId, r.Id }).IsClustered(true);
            });//Use a hybrid clusted index for Hotel,Room. we will never be looking for a room without knowing the hotel.

            //Note this is not the correct index setup for optimal performance, its akward to set this up via EF and would be easier to apply directly to the database
            //If I had more time id define the many to many table specifically through the DBContext so i could reference its fields.
            //The main performance index should really be (RoomID,DepartureDate,ArrivalDate) the reason departure date is first is because over time
            //the majority of bookings will be in the past.
            modelBuilder.Entity<Booking>().HasIndex(b => new { b.DepartureDate, b.ArrivalDate }).IsClustered(false);//Indexes are important for performance when doing booking checks and Reference Lookups
            modelBuilder.Entity<Booking>().HasIndex(b => new { b.Reference }).IsClustered(false);//Non clustered index grants performance for checking room availability, while letting the clustered index handle lookups
            modelBuilder.Entity<Hotel>().HasIndex(b => new { b.Name }).IsClustered(false);//For quick search by name
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DBString);
            }
        }


    }
}
