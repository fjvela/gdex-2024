namespace GloboTicket.Catalog;

public record EventRecommendationsSettings
{
    public string ModelName { get; init; }
    public string Endpoint { get; init; }

    public string ApiKey { get; set; }
}