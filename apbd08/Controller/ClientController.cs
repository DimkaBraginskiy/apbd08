using apbd08.Model;
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


    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientsAsync(CancellationToken token, int Id)
    {
        var clients = await _clientService.GetTripsByClientIdAsync(token, Id);
        return Ok(clients);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClientAsync(CancellationToken token, [FromBody] Client client)
    {
        if (client == null)
        {
            return BadRequest("Client cannot be null");
        }

        var id = await _clientService.AddClientAsync(token, client);
        return CreatedAtAction(nameof(GetClientsAsync), new { id }, client);
    }
}