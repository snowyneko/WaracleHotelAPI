using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Waracle_HotelAPI;
using Waracle_HotelAPI.Interfaces;
using Waracle_HotelAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<BookingDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "Waracle Hotel API";
    options.Theme = ScalarTheme.Purple;
    options.ShowSidebar = true;
});




app.MapControllers();

app.Run();
