using apbd08.Model;

namespace apbd08.Service;

public interface IClientService
{
    public Task<List<Trip>> GetTripsByClientIdAsync(CancellationToken token, int id);
    public Task<int> AddClientAsync(CancellationToken token, Client client);
    public Task<Client?> GetClientByIdAsync(CancellationToken token, int id);
    public Task AssignClientToTripAsync(CancellationToken token, int clientId, int tripId);
}