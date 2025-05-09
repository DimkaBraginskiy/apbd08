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


    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientsAsync(CancellationToken token, int id)
    {
        var clients = await _clientService.GetTripsByClientIdAsync(token, id);
        return Ok(clients);
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
    
    [HttpPost]
    public async Task<IActionResult> AddClientAsync(CancellationToken token, [FromBody] ClientCreateDto clientDto)
    {
        if (clientDto == null)
        {
            return BadRequest("Client cannot be null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var client = new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Email = clientDto.Email,
            Telephone = clientDto.Telephone,
            Pesel = clientDto.Pesel
        };
        
        
        var id = await _clientService.AddClientAsync(token, client);
        
        var response = new ClientResponseIdDto { IdClient = id };
        return CreatedAtRoute("GetClientById", new { id }, response);
    }
}