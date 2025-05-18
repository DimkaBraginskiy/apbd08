using apbd08.Model;
using apbd08.Model.DTOs;
using apbd08.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace apbd08.Controller;

[Route("api/clients")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    // Method for getting all trips which are associated with the client (it's provided id):
    //***not sure whether should I use here another dto identically the same as Model...
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsAsync(CancellationToken token, int id) 
    {
        var client = await _clientService.GetClientByIdAsync(token, id);
        if (client == null)
            return NotFound($"Client with ID {id} does not exist.");

        var trips = await _clientService.GetTripsByClientIdAsync(token, id);
        if (trips.Count == 0)
            return NotFound($"Client with ID {id} is not registered for any trips.");

        return Ok(trips);
    }
    
    [HttpGet("{id}", Name = "GetClientById")]
    public async Task<IActionResult> GetClientAsync(CancellationToken token, int id)
    {
        var client = await _clientService.GetClientByIdAsync(token, id);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }
    
    // Mehtod for adding a new Client to the database using DTO to avoid unnecessary fields which are not mentioned in the task:
    [HttpPost]
    public async Task<IActionResult> AddClientAsync(CancellationToken token, [FromBody] ClientRequestDto clientDto)
    {
        if (clientDto == null)
        {
            return BadRequest("Client cannot be null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _clientService.AddClientAsync(token, clientDto);
        
        return CreatedAtRoute("GetClientById", new { id = response.IdClient }, response);
    }

    // Assigning a Client to a Trip by passing id of the Client and if of the Trip:
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> AssignClientToTripAsync(CancellationToken token, [FromRoute] int id, [FromRoute] int tripId)
    {
        
        try
        {
            var registeredAt = await _clientService.AssignClientToTripAsync(token, id, tripId);

            var response = new ClientTripAssignResponseDto()
            {
                IdClient = id,
                IdTrip = tripId,
                RegisteredAt = registeredAt
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
    
    // Method for Client removal from the Trip based on provided IdTrip and IdClient:
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTripAsync(CancellationToken token, [FromRoute] int id, [FromRoute] int tripId)
    {
        var succeeded = await _clientService.RemoveClientFromTripAsync(token, id, tripId);
        if (!succeeded)
        {
            return NotFound();
        }
        return Ok(new
        {
            Message = "Client removed from trip successfully",
            IdClient = id,
            IdTrip = tripId,
            RemovedAt = DateTime.Now
        });
    }
}