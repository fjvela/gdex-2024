namespace GloboTicket.Catalog.Infrastructure;

internal static class Database
{
    static Database()
    {
        LoadSampleData();
    }
    
    public static List<Event> Events { get; } = new();
    public static List<Artist> Artists { get; } = new();
    
    public static void LoadSampleData()
    {
        Events.Clear();
        Artists.Clear();
        
        var je = new Artist(Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA320}"),"ss", "aa");
        var ev = new Event(je, new DateTime(), "SSS", Guid.NewGuid(), "SS", "AA", new decimal(11), "DD");

        var johnEgbert = new Artist(Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA320}"), "John Egbert", "Banjo virtuoso");
        var michaelJohnson = new Artist(Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA322}"), "Michael Johnson", "Stand up comedian");
        var nickSailor = new Artist(Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA321}"), "Nick Sailor", "Playwright");

        Artists.Add(johnEgbert);
        Artists.Add(michaelJohnson);
        Artists.Add(nickSailor);
        Events.Add(ev);

        Events.Add(new Event(
            johnEgbert,
            DateTime.Now.AddMonths(6),
            "Join John for his farewell tour across 15 continents. John really needs no introduction since he has already mesmerized the world with his banjo.",
            Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA317}"),
            "/img/banjo.jpg",
            "John Egbert Live",
            65,
            "Yankee Stadium"));

        //add an event to the list
        Events.Add(new Event(
            nickSailor,
            DateTime.Now.AddMonths(8),
            "The critics are over the moon and so will you after you've watched this sing and dance extravaganza written by Nick Sailor, the man from 'My dad and sister'.",
            Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA318}"),
            "/img/musical.jpg",
            "To the Moon and Back",
            135,
            "Arena NY"));
        
    }
}