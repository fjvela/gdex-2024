using Microsoft.AspNetCore.Mvc;
using GloboTicket.Catalog.Repositories;
using System.Threading.Tasks;

namespace GloboTicket.Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventRepository eventRepository, ILogger<EventController> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        // GET: api/Event
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return Ok(await _eventRepository.GetEvents());
        }

        // POST: api/Event
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            if (!request.IsValid)
                return BadRequest();

            var @event = request.ToEvent();

            try
            {
                await _eventRepository.Save(@event);
                return CreatedAtRoute("GetById", new { id = @event.EventId }, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event");
                return StatusCode(500);
            }
        }

        // GET: api/Event/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var evt = await _eventRepository.GetEventById(id);

            if (evt == null)
            {
                return NotFound();
            }

            return Ok(evt);
        }
    }
}