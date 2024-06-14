namespace GloboTicket.Catalog.Controllers;

public record CreateEventRequest(
    string Name,
    int Price,
    Artist Artist,
    DateTime Date,
    string Description,
    string? ImageUrl,
    string Venue)
{
    public bool IsValid
        => !string.IsNullOrWhiteSpace(Name)
           && Price > 0
           && Artist.IsValid
           && Date > DateTime.MinValue
           && !string.IsNullOrWhiteSpace(Description)
           && !string.IsNullOrWhiteSpace(Venue);

    public Event ToEvent()
        => new(Artist,
               Date,
               Description,
               Guid.NewGuid(),
               ImageUrl,
               Name,
               Price,
               Venue);
}