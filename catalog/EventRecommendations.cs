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
        
    }
}