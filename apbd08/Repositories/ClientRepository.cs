using apbd08.Model;
using Microsoft.Data.SqlClient;

namespace apbd08.Repositories;

public class ClientRepository
{
    string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";


    public async Task<List<Trip>> GetTripsAsync(CancellationToken token, int IdClient)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(token);
            var command = new SqlCommand(@"SELECT Trip.*, Country.Name AS NameCountry, CT.RegisteredAt, CT.PaymentDate FROM trip
JOIN Country_Trip ON trip.IdTrip = Country_Trip.IdTrip
JOIN COUNTRY ON Country_Trip.IdCountry = COUNTRY.IdCountry
JOIN Client_Trip CT on trip.IdTrip = CT.IdTrip
WHERE CT.IdClient = @IdClient", connection);
            command.Parameters.AddWithValue("@IdClient", IdClient);
            var reader = await command.ExecuteReaderAsync(token);
            var trips = new List<Trip>();
            while (await reader.ReadAsync(token))
            {
                var found = trips.FirstOrDefault(t => t.IdTrip == reader.GetInt32(reader.GetOrdinal("IdTrip")));
                if (found == null)
                {
                    var trip = new Trip
                    {
                        IdTrip = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        Countries = new List<Country>(
                            new[]
                            {
                                new Country
                                {
                                    Name = reader.GetString(reader.GetOrdinal("NameCountry"))
                                }
                            })
                    };
                    trips.Add(trip);    
                }
                else
                {
                    var country = new Country
                    {
                        Name = reader.GetString(reader.GetOrdinal("NameCountry"))
                    };
                    found.Countries.Add(country);
                }
            }
            return trips;
        }
    }


    public async Task<int> AddClientAsync(CancellationToken token, Client client)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(token);
            var command = new SqlCommand(@"insert into client (FirstName, LastName, Email, Telephone, Pesel)
values (@FirstName, @LastName, @Email, @Telephone, @Pesel)");

            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);

            var result = await command.ExecuteNonQueryAsync(token);

            return Convert.ToInt32(result);
        }
    }
}