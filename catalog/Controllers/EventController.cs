using Microsoft.AspNetCore.Mvc;
using GloboTicket.Catalog.Repositories;

namespace GloboTicket.Catalog.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventRepository _eventRepository;

    private static int callcounter = 0;
    private readonly ILogger<EventController> _logger;

    public EventController(IEventRepository eventRepository, ILogger<EventController> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetEvents")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _eventRepository.GetEvents());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        if(!request.IsValid)
            return BadRequest();
    
        var @event = request.ToEvent();
    
        try
        {
            await _eventRepository.Save(@event);
            return CreatedAtRoute("GetById", new {id = @event.EventId}, request);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error saving event");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(Guid id)
    {        
        var evt = await _eventRepository.GetEventById(id);
        return Ok(evt);
    }
}
