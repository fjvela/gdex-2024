using System.Text.Json;

using Azure.Data.Tables;

namespace GloboTicket.Catalog.Repositories;

public class AzureStorageEventRepository : IEventRepository
{
    private const string partitionKey = "global";
    private readonly TableClient _tableClient;
    private readonly ILogger<AzureStorageEventRepository> logger;

    public AzureStorageEventRepository(ILogger<AzureStorageEventRepository> logger, IConfiguration configuration)
    {
        this.logger = logger;

        var connectionString = configuration.GetConnectionString("EventsConnection") ??
                               throw new InvalidOperationException("Missing connection string");
        //var storageUri = @"https://stdistractedjang.table.core.windows.net/table?se=2024-12-31&sp=a&sv=2019-02-02&tn=table&sig=IlyxYg67EIy2GVUxZ3KWfYY3R1MyGZYLcrFt8EAUXtE%3D";
        var tableName = "event";

        var serviceClient = new TableServiceClient(connectionString);
        _tableClient = serviceClient.GetTableClient(tableName);
    }

    public async Task<Event> GetEventById(Guid eventId)
    {
        var entity = await _tableClient.GetEntityAsync<TableEntity>(partitionKey, eventId.ToString());

        if(entity == null || entity.HasValue == false)
        {
            throw new InvalidOperationException("Event not found");
        }

        entity.Value.TryGetValue("content", out var content);

        return JsonSerializer.Deserialize<Event>(content.ToString());
    }

    public async Task Save(Event @event)
    {
        var entity = new TableEntity(partitionKey, @event.EventId.ToString()) {{"content", JsonSerializer.Serialize(@event)}};
        await _tableClient.AddEntityAsync(entity);
    }

    public void UpdateSpecialOffer()
    {
    }

    public Task<IEnumerable<Artist>> GetArtists()
    {
        return null;
    }

    public Task<Artist> AddArtist(string name, string genre) => null;

    public async Task<IEnumerable<Event>> GetEvents()
    {
        string filter = $"PartitionKey eq '{partitionKey}'";
        var entities = _tableClient.QueryAsync<TableEntity>(filter);
        List<Event> events = new List<Event>();

        await foreach(var entity in entities)
        {
            entity.TryGetValue("content", out var content);

            Event eventObj = JsonSerializer.Deserialize<Event>(content.ToString());
            events.Add(eventObj);
        }

        return events;
    }
}