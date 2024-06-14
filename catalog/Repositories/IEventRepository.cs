using GloboTicket.Catalog.Controllers;

namespace GloboTicket.Catalog.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetEvents();
    Task<Event> GetEventById(Guid eventId);
    Task Save(Event @event);
}
