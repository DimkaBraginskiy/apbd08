using apbd08.Model;
using apbd08.Model.DTOs;
using apbd08.Repositories;

namespace apbd08.Service;

public class ClientService : IClientService
{
    private ClientRepository ClientRepository;
    
    public ClientService(ClientRepository clientRepository)
    {
        ClientRepository = clientRepository;
    }
    
    public async Task<List<ClientTripDto>> GetTripsByClientIdAsync(CancellationToken token, int Id)
    {
        var clients = await ClientRepository.GetTripsAsync(token, Id);
        return clients;   
    }
    
    public async Task<int> AddClientAsync(CancellationToken token, Client client)
    {
        var id = await ClientRepository.AddClientAsync(token, client);
        return id;
    }
    
    public async Task<Client?> GetClientByIdAsync(CancellationToken token, int id)
    {
        var client = await ClientRepository.GetClientByIdAsync(token, id);
        return client;
    }
    
    public async Task AssignClientToTripAsync(CancellationToken token, int clientId, int tripId)
    {
        var succeeded = await ClientRepository.AssignClientToTripAsync(token, clientId, tripId);
        if (!succeeded)
        {
            throw new Exception("Failed to assign client to trip.");
        }
    }

    public async Task<bool> RemoveClientFromTripAsync(CancellationToken token,  int clientId, int tripId)
    {
        var succeeded = await ClientRepository.RemoveClientFromTripAsync(token,  clientId, tripId);
        if (!succeeded)
        {
            throw new Exception("Failed to remove client from trip.");
        }
        return succeeded;
    }
}