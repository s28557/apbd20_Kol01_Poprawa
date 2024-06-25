using Kol01.Models;
using Kol01.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Kol01.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;
    public ClientsController(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        if (!await _clientRepository.DoesClientExist(id))
            return NotFound($"Client with given ID - {id} doesn't exist");

        var res = await _clientRepository.GetClient(id);

        return Ok(res);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddNewClient(NewClientDto newClientDto)
    {
        
        var res = await _clientRepository.AddNewClient(newClientDto);
        
        return Created(Request.Path.Value ?? "api/clients", newClientDto);
    }
    
}