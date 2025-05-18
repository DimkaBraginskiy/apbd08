using apbd08.Model.DTOs;
using apbd08.Repositories;

namespace apbd08.Service;

public class TripService : ITripService
{
    private TripRepository _tripRepository;

    public TripService(TripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<List<TripResponseDto>> GetTripsAsync(CancellationToken token)
    {
        var trips = await _tripRepository.GetTripsAsync(token);
        return trips;
    }
}