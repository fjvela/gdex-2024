namespace GloboTicket.Catalog;

public record Event(Artist Artist,
                    DateTime Date,
                    string Description,
                    Guid EventId,
                    string ImageUrl,
                    string Name,
                    decimal Price,
                    string Venue);