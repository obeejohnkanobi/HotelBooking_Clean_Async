using System.Net;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Composition root: infrastructure choices are made here so Core remains
// framework-agnostic and unit-test friendly.
//SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
// In-memory database only exists while the connection is open
//connection.Open();
builder.Services.AddDbContext<HotelBookingContext>(opt => 
    opt.UseInMemoryDatabase("HotelBookingDb"));

// Dependency Inversion in action: business services depend on abstractions,
// while the application layer wires those abstractions to concrete adapters.
builder.Services.AddScoped<IRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Booking>, BookingRepository>();
builder.Services.AddScoped<IBookingManager, BookingManager>();
builder.Services.AddTransient<IDbInitializer, DbInitializer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed known data for reproducible local behavior and demo scenarios.
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetService<HotelBookingContext>();
        var dbInitializer = services.GetService<IDbInitializer>();
        dbInitializer.Initialize(dbContext);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
