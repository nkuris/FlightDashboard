using FlightDashboard.Server.Data;
using Microsoft.EntityFrameworkCore;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
// Add SignalR
builder.Services.AddSignalR();


// Add services to the container.

builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]),
    ServiceLifetime.Scoped);
builder.Services.AddScoped<FlightDashboard.Server.Interfaces.IFlightService, FlightDashboard.Server.Services.FlightService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name:MyAllowSpecificOrigins, builder =>
//    {
//        builder.AllowAnyMethod()
//        .AllowAnyHeader()
//        .SetIsOriginAllowed(origin => true) // allow any origin
//        .AllowCredentials(); // allow credentials
//    });
//});


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS before routing
app.UseCors( x => x
       .AllowAnyMethod()
       .AllowAnyHeader()
       .SetIsOriginAllowed(origin => true) // allow any origin
       .AllowCredentials()); // allow credentials
app.UseAuthorization();

app.MapControllers();

// Map SignalR hub (create FlightHub class separately)
app.MapHub<FlightDashboard.Server.Hubs.FlightHub>("/hubs/flight");

app.MapFallbackToFile("/index.html");

app.Run();
