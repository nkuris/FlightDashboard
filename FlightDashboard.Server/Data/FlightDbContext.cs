using FlightDashboard.Server.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FlightDashboard.Server.Data
{
    public class FlightDbContext : DbContext
    {
        public string DbPath { get; }

        public FlightDbContext() :
            base()
        {
            var folder = Environment.GetEnvironmentVariable("ConnectionStrings:SQLiteDefault");
            DbPath = System.IO.Path.Join(folder, "Flights.db");
        }

        public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
        {
            try
            {
                var folder = Environment.GetEnvironmentVariable("ConnectionStrings:SQLiteDefault");
                DbPath = System.IO.Path.Join(folder, "Flights.db");
               // SqliteConnection sqliteConnection = new SqliteConnection();
               // sqliteConnection.ConnectionString = $"Data Source={DbPath}";
               // sqliteConnection.Open();
                //Database.EnsureCreated();
                //Database.Migrate();
            }
            catch(Exception ex)
            {

            }
        }

        // Add DbSet properties for Flights and FlightStatus
        public DbSet<Flight> Flights { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
            

        // CRUD for Flights
        public async Task<List<Flight>> GetAllFlightsAsync()
        {

            try
            {
                return await Flights.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, log them, etc.
                throw new Exception("Error retrieving flights", ex);
            }
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            try
            {
                return await Flights.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log them, etc.
                throw new Exception($"Error retrieving flight with ID {id}", ex);
            }
        }

        public async Task<Flight> AddFlightAsync(Flight flight)
        {
            try
            {
                Flights.Add(flight);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, log them, etc.
                throw new Exception("Error adding flight", ex);
            }
            return flight;
        }

        public async Task<bool> UpdateFlightAsync(Flight flight)
        {
            bool res = true;

            try
            {
                var existing = await Flights.FirstOrDefaultAsync(x => x.Id == flight.Id);
                if (existing == null)
                    res = false;

                Flights.Update(flight);
                await SaveChangesAsync();
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public async Task<bool> DeleteFlightAsync(int id)
        {
            bool res = false;
            var flight = await Flights.FindAsync(id);
            if (flight != null)
            {
                Flights.Remove(flight);
                await SaveChangesAsync();
            }
            else
            {
                res = false;
            }
            return res;
        }

        // CRUD for FlightStatus
        
    }
}
