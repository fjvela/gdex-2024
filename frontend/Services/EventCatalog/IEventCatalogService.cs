using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GloboTicket.Catalog.Controllers;
using GloboTicket.Frontend.Models.Api;

namespace GloboTicket.Frontend.Services
{
    public interface IEventCatalogService
    {
        Task<IEnumerable<Event>> GetAll();

        Task<Event> GetEvent(Guid id);

        Task CreateEvent(CreateEventRequest createEventRequest);
    }
}
