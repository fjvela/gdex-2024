using GloboTicket.Catalog.Infrastructure;

namespace GloboTicket.Catalog.Repositories;

public class InMemoryEventRepository : IEventRepository
{
    private readonly ILogger<InMemoryEventRepository> logger;

    public InMemoryEventRepository(ILogger<InMemoryEventRepository> logger)
    {
        this.logger = logger;
    }

    public Task<IEnumerable<Event>> GetEvents()
    {
        // this just returning an in-memory list for now
        return Task.FromResult((IEnumerable<Event>)Database.Events);
    }

    public Task<Event> GetEventById(Guid eventId)
    {
        var @event = Database.Events.FirstOrDefault(e => e.EventId == eventId);
        if (@event == null)
        {
            throw new InvalidOperationException("Event not found");
        }
        return Task.FromResult(@event);
    }

    public Task Save(Event @event)
    {
        Database.Events.Add(@event);
        return Task.CompletedTask;
    }

    // scheduled task calls this periodically to put one random item on special offer
    public void UpdateSpecialOffer()
    {
        Database.LoadSampleData();
        // pick a random one to put on special offer
        var random = new Random();
        var specialOfferEvent = Database.Events[random.Next(0, Database.Events.Count)];
    }

    public Task<Artist> AddArtist(string name, string genre)
    {
        var artist = new Artist(Guid.NewGuid(), name, genre);
        Database.Artists.Add(artist);
        return Task.FromResult(artist);
    }

    public Task<IEnumerable<Artist>> GetArtists()
    {
        return Task.FromResult((IEnumerable<Artist>)Database.Artists);
    }
}
