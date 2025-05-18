using apbd08.Model.DTOs;

namespace apbd08.Service;

public interface ITripService
{
    public Task<List<TripResponseDto>> GetTripsAsync(CancellationToken token);
}