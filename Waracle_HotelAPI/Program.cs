using Microsoft.EntityFrameworkCore;
using Waracle_HotelAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}





app.MapControllers();

app.Run();
