using System.Net.Http.Json;
using GloboTicket.Catalog.Controllers;

namespace eventImporter.Catalog;

public class CatalogClient(HttpClient client)
{
    public async Task Save(CreateEventRequest request)
    {
        var response = await client.PostAsJsonAsync("Event", request);
        response.EnsureSuccessStatusCode();
    }
}