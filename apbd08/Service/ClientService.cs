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
    
    public async Task<ClientResponseDto> AddClientAsync(CancellationToken token, ClientRequestDto clientDto)
    {
        var client = new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        };
        
        var id = await ClientRepository.AddClientAsync(token, client);

        return new ClientResponseDto { IdClient = id };
    }
    
    public async Task<Client?> GetClientByIdAsync(CancellationToken token, int id)
    {
        var client = await ClientRepository.GetClientByIdAsync(token, id);
        return client;
    }
    
    public async Task<DateTime> AssignClientToTripAsync(CancellationToken token, int clientId, int tripId)
    {
        var registeredAt = await ClientRepository.AssignClientToTripAsync(token, clientId, tripId);
        return registeredAt;
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