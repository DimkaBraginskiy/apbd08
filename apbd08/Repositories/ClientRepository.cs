using apbd08.Model;
using Microsoft.Data.SqlClient;

namespace apbd08.Repositories;

public class ClientRepository
{
    string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    //Method for getting all trips for a given client id:
    public async Task<List<Trip>> GetTripsAsync(CancellationToken token, int IdClient)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            
            // Selecting all values of trips, names of countries, and two additional columns from Client_Trip table on the condition that the client id exists in Client_Trip table:
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

    // Method for adding a new Client to the database:
    public async Task<int> AddClientAsync(CancellationToken token, Client client)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(token);
            var command = new SqlCommand(@"insert into client (FirstName, LastName, Email, Telephone, Pesel)
OUTPUT INSERTED.IdClient
values (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection);

            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);

            var result = await command.ExecuteScalarAsync(token);

            return Convert.ToInt32(result);
        }
    }

    // Method for getting a single Client by IdClient (it's id):
    public async Task<Client?> GetClientByIdAsync(CancellationToken token, int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            // Selecting everything from Client table based on provided id:
            await connection.OpenAsync(token);
            var command = new SqlCommand(@"SELECT * FROM Client WHERE IdClient = @IdClient", connection);
            command.Parameters.AddWithValue("@IdClient", id);
            var reader = await command.ExecuteReaderAsync(token);
            if (await reader.ReadAsync(token))
            {
                return new Client
                {
                    IdClient = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Telephone = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
                };
            }
            return null;
        }
    }
    
    // Method for assignment to a Client_Trip table values: IdClient, IdTrip, RegisteredAt:
    public async Task<bool> AssignClientToTripAsync(CancellationToken token, int idClient, int idTrip)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(token);
            
            // Checking whether the client exists:
            var clientExistsCommand = new SqlCommand(@"SELECT 1 FROM Client WHERE IdClient = @IdClient", connection);
            clientExistsCommand.Parameters.AddWithValue("@IdClient", idClient);
            if(await clientExistsCommand.ExecuteScalarAsync(token) == null)
            {
                throw new ArgumentException("Client does not exist.");
            }
            
            
            // Checking whether the trip exists:
            var tripExistsCommand = new SqlCommand(@"SELECT MaxPeople FROM Trip WHERE IdTrip = @IdTrip", connection);
            tripExistsCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            var maxPeopleObj = await tripExistsCommand.ExecuteScalarAsync(token);
            if (maxPeopleObj == null)
            {
                throw new ArgumentException("Trip does not exist.");
            }

            int maxPeople = Convert.ToInt32(maxPeopleObj);
            
            //Checking whether the provided ids have not been already assigned to the table:
            
            var alreadyAssignedCommand = new SqlCommand(@"SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", connection);
            alreadyAssignedCommand.Parameters.AddWithValue("@IdClient", idClient);
            alreadyAssignedCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            if (await alreadyAssignedCommand.ExecuteScalarAsync(token) != null)
            {
                throw new ArgumentException("Client is already assigned to this trip.");
            }
            
            //Checking current participant count and checking if the trip is not full of people:
            var countCommand = new SqlCommand(@"SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @IdTrip", connection);
            countCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            var currentCountObj = await countCommand.ExecuteScalarAsync(token);
            int currentCount = Convert.ToInt32(currentCountObj);
            if (currentCount >= maxPeople)
            {
                throw new ArgumentException("Trip is already full.");
            }
            
            
            //After all checks Inserting:
            
            var insertCommand = new SqlCommand(@"INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) 
VALUES (@IdClient, @IdTrip, @RegisteredAt)", connection);
            insertCommand.Parameters.AddWithValue("@IdClient", idClient);
            insertCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            insertCommand.Parameters.AddWithValue("@RegisteredAt", GetCurrentDateAsInt());
            
            int affected = await insertCommand.ExecuteNonQueryAsync(token);
            return affected > 0;
        }
    }
    //Method for converting DateTime to int in YYYYMMDD format
    private int GetCurrentDateAsInt()
    {
        DateTime currentDate = DateTime.Today;
        return currentDate.Year * 10000 + currentDate.Month * 100 + currentDate.Day;
    }
    
    public async Task<bool> RemoveClientFromTripAsync(CancellationToken token, int idClient, int idTrip)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync(token);
            
            
            //Checking whether the registration even exists:
            var registrationExistsCommand = new SqlCommand(@"SELECT 1 FROM Client_Trip 
         WHERE IdClient = @IdClient AND IdTrip = @IdTrip", connection);
            registrationExistsCommand.Parameters.AddWithValue("@IdClient", idClient);
            registrationExistsCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            if (await registrationExistsCommand.ExecuteScalarAsync(token) == null)
            {
                throw new ArgumentException("Client is not registered for any trips.");
            }
            
            
            // If everything is ok, deleting the client from the trip:

            var deleteClientCommand =
                new SqlCommand(@"DELETE FROM Client_Trip 
                WHERE IdClient = @IdClient AND IdTrip = @IdTrip", connection);
            deleteClientCommand.Parameters.AddWithValue("@IdClient", idClient);
            deleteClientCommand.Parameters.AddWithValue("@IdTrip", idTrip);
            
            var affectedRows = await deleteClientCommand.ExecuteNonQueryAsync(token);
            return affectedRows > 0;
        }
    }
}