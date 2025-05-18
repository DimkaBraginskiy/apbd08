using apbd08.Model;
using apbd08.Model.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd08.Repositories;

public class TripRepository
{
    private readonly string _connectionString;

    public TripRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    
    
    public async Task<List<TripResponseDto>> GetTripsAsync(CancellationToken token)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(token);
            var command = new SqlCommand(@"SELECT Trip.*, Country.Name AS NameCountry FROM trip
JOIN Country_Trip ON trip.IdTrip = Country_Trip.IdTrip
JOIN COUNTRY ON Country_Trip.IdCountry = COUNTRY.IdCountry", connection);
             
            var reader = await command.ExecuteReaderAsync(token);
            var trips = new List<TripResponseDto>();
            while (await reader.ReadAsync(token))
            {
                var found = trips.FirstOrDefault(t => t.IdTrip == reader.GetInt32(reader.GetOrdinal("IdTrip")));
                if (found == null)
                {
                    var trip = new TripResponseDto()
                    {
                        IdTrip = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        Countries = new List<CountryResponseDto>
                        {
                            new CountryResponseDto
                            {
                                Name = reader.GetString(reader.GetOrdinal("NameCountry"))
                            }
                        }
                    };
                    trips.Add(trip);    
                }
                else
                {
                    var country = new CountryResponseDto()
                    {
                        Name = reader.GetString(reader.GetOrdinal("NameCountry"))
                    };
                    found.Countries.Add(country);
                }
            }
            return trips;
        }
    }
}