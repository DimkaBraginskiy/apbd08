using apbd08.Repositories;
using apbd08.Service;
using Microsoft.AspNetCore.Mvc;

namespace apbd08.Controller;

[Route("api/trips")]
[ApiController]
public class TripController : ControllerBase
{
    private ITripService _tripService;

    public TripController(ITripService tripService)
    {
        _tripService = tripService;
    }
    
    
    // Method for getting all trips from the database:
    [HttpGet]
    public async Task<IActionResult> GetTrips(CancellationToken token)
    {
        return Ok(await _tripService.GetTripsAsync(token));
    }
}