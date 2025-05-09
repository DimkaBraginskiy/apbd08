using apbd08.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace apbd08.Controller;

[Route("api/trips")]
[ApiController]
public class TripController : ControllerBase
{
    private TripRepository _tripRepository;

    public TripController(TripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetTrips(CancellationToken token)
    {
     
        return Ok(await _tripRepository.GetTripsAsync(token));
    }
}