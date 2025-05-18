using apbd08.Model;
using apbd08.Model.DTOs;

namespace apbd08.Service;

public interface IClientService
{
    public Task<List<ClientTripDto>> GetTripsByClientIdAsync(CancellationToken token, int Id);
    public Task<ClientResponseDto> AddClientAsync(CancellationToken token, ClientRequestDto clientDto);
    public Task<Client?> GetClientByIdAsync(CancellationToken token, int id);
    public Task AssignClientToTripAsync(CancellationToken token, int clientId, int tripId);
    public Task<bool> RemoveClientFromTripAsync(CancellationToken token,  int clientId, int tripId);
}