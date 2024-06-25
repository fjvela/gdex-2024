using eventImporter.Catalog;

namespace eventImporter;

internal class PeriodicEventFileImporter(
    EventFileParser eventFileParser,
    CatalogClient catalogClient,
    string[] filesToParse)
{
    private readonly HttpClient _httpClient = new();

    public async Task RunAsync()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
        try
        {
            await ExecuteTask();

            while(await timer.WaitForNextTickAsync())
            {
                await ExecuteTask();
            }
        }
        catch(OperationCanceledException)
        {
        }
    }

    private async Task ExecuteTask()
    {
        foreach(var eventFile in filesToParse)
        {
            var fileContent = await _httpClient.GetStringAsync(eventFile);
            await foreach(var @event in eventFileParser.Parse(fileContent))
            {
                await catalogClient.Save(@event);
            }
        }
    }
}