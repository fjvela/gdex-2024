namespace GloboTicket.Catalog;

public record Artist(
    Guid Id,
    string Name,
    string Genre)
{
    public bool IsValid => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Genre);
}