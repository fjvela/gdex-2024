using System.Text.Json;

using GloboTicket.Catalog.Repositories;

using Microsoft.SemanticKernel;

namespace GloboTicket.Catalog;

public class EventRecommendations
{
    private readonly IEventRepository _eventRepository;
    private readonly Kernel _kernel;

    public EventRecommendations(EventRecommendationsSettings settings,
                                IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;

        _kernel = Kernel.CreateBuilder()
                        .AddAzureOpenAIChatCompletion(deploymentName:settings.ModelName,
                                                      endpoint:settings.Endpoint,
                                                      apiKey:settings.ApiKey)
                        .Build();
    }

    public async Task<IEnumerable<Event>> GetRecommendations(string artist)
    {
        var now = DateTime.Now;
        var events = await _eventRepository.GetEvents();

        var availableEvents = events.Where(e => e.Date > now && e.Date < now.AddMonths(1)).ToList();

        const string prompt = "Given the artist {{$input_artist}}, recommend similar artists from the following events: {{$event_list}}. " +
                              "ONLY return the event id's of the recommended events as a comma separated list nothing else";
        var arguments = new KernelArguments
                        {
                            {"input_artist", artist},
                            {"event_list", string.Join(Environment.NewLine, availableEvents.Select(e => JsonSerializer.Serialize(e)))}
                        };
        var result = await _kernel.InvokePromptAsync(prompt, arguments);
        var chosenEvents = result.ToString()
                                 .Split(",")
                                 .Select(eventId => eventId.Trim())
                                 .Select(Guid.Parse);
        return availableEvents.Where(e => chosenEvents.Contains(e.EventId)).ToList();
    }
}