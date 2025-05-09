using apbd08.Model;

namespace apbd08.Service;

public interface IClientService
{
    public Task<List<Trip>> GetTripsByClientIdAsync(CancellationToken token, int Id);
    public Task<int> AddClientAsync(CancellationToken token, Client client);
}