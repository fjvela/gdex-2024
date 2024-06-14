using System.ComponentModel;
using System.Text.Json;

using GloboTicket.Catalog.Controllers;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace eventImporter;

public class EventFileParser
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly Kernel _kernel;

    public EventFileParser(IKernelBuilder kernelBuilder)
    {
        kernelBuilder.Plugins.AddFromType<CurrencyPlugin>();
        _kernel = kernelBuilder.Build();

        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async IAsyncEnumerable<CreateEventRequest> Parse(string eventFile)
    {
        var chunks = await Chunk(eventFile);

        foreach(var chunk in chunks.AsParallel())
        {
            yield return await ParseEvent(chunk);
        }
    }

    private async Task<CreateEventRequest> ParseEvent(string text)
    {
        var history = new ChatHistory
                      {
                          new(AuthorRole.System,
                              """
                              You are tasked with converting a user's description of a music event into a structured JSON format.
                              Only the description provided in the latest user input should be processed into the output. Ignore all previous interactions and outputs.
                              Follow this template:
                              {
                                  "ContextId": "the id of the question asked by the user",
                                  "Artist": {
                                      "Name": "extracted artist name",
                                      "Genre": "extracted genre, if available",
                                  },
                                  "Name": "extracted event name",
                                  "Venue": "extracted event location",
                                  "Date": "date in YYYY-MM-DD format",
                                  "Description": "concise event description",
                                  "Price": extracted price as integer converted to dollar
                              }
                              """),
                          new(AuthorRole.User,
                              """In the heart of the city's pulse, on the imminent 10th of September, 2024, amidst the hollowed grounds of Soldier Field under the mesmerizing guise of midnight, there unfolds an ethereal spectacle - 'Nightfall Nexus', an impeccable cosmic symphony crafted by none other than the celestial artisan, Earth Wind & Fire, for the privileged witnesses able to spare a sum of $121."""),
                          new(AuthorRole.Assistant,
                              """
                              {
                                  "Artist": {
                                      "Name": "Earth Wind & Fire",
                                      "Genre": null
                                  },
                                  "Name": "Nightfall Nexus",
                                  "Venue": "Soldier Field",
                                  "Date": "2024-09-10",
                                  "Description": "an impeccable cosmic symphony crafted by none other than the celestial artisan, Earth Wind & Fire",
                                  "Price": 121
                              }
                              """),
                          new(AuthorRole.User, text)
                      };

        try
        {
            var settings = new OpenAIPromptExecutionSettings {ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions};
            var result = await _chatCompletionService.GetChatMessageContentAsync(history, settings, _kernel);

            var parsedResult = JsonSerializer.Deserialize<Event>(result.ToString());
            return parsedResult.AsEvent();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }
    }

    private async Task<IEnumerable<string>> Chunk(string allText)
    {
        var history = new ChatHistory
                      {
                          new(AuthorRole.System,
                              """
                              You are tasked with splitting a large text into individual blocks, each describing a single music event. Below is the text content from a file:

                              ### Tasks:

                              1. Ensure no information is omitted. Include all text as it appears in the file.
                              2. Produce the output in valid JSON format. The output must be directly parsable into an Array of Strings, each string representing a single event's description.
                              """),
                          new(AuthorRole.User,
                              """
                              In the heart of the city's pulse, on the imminent 10th of September, 2024, amidst the hollowed grounds of Soldier Field under the mesmerizing guise of midnight, there unfolds an ethereal spectacle - 'Nightfall Nexus', an impeccable cosmic symphony crafted by none other than the celestial artisan, Earth Wind & Fire, for the privileged witnesses able to spare a sum of $121.
                              """),
                          new(AuthorRole.Assistant,
                              """
                              [
                                  "Event description text for the first event"
                              ]
                              """),
                          new(AuthorRole.User, allText)
                      };

        try
        {
            var result = await _chatCompletionService.GetChatMessageContentAsync(history, new OpenAIPromptExecutionSettings(), _kernel);
            return JsonSerializer.Deserialize<IEnumerable<string>>(result.ToString());
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    private class CurrencyPlugin
    {
        [KernelFunction,
         Description("Currency amount and returns a random amount in USD")]
        [return:Description("A dollar equivalent of the provided currency amount")]
        public static int ConvertToDollar([Description("The ISO 4217 currency code")] string currencyCode,
                                          [Description("The amount of money to convert")]
                                          decimal amount)
        {
            return 100;
        }
    }

    private record Artist
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Id { get; set; }

        public GloboTicket.Catalog.Artist AsArtist()
            => new(Guid.NewGuid(), Name, Genre);
    }

    private record Event
    {
        public Artist Artist { get; set; }
        public string Name { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public CreateEventRequest AsEvent()
            => new(Name,
                   Price,
                   Artist.AsArtist(),
                   Date,
                   Description,
                   null,
                   Venue);
    }
}