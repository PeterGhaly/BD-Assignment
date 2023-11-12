using BD_Assignment.Dtos;
using BD_Assignment.Models;

namespace BD_Assignment.Services
{
    public interface IEventSevice
    {
        public ServiceResponse<List<CalenderEvent>> GetEvents(DateTime? startDate, DateTime? endDate, string searchQuery);
        public ServiceResponse<CalenderEvent> AddEvent(AddEventDto eventDto);
        public Task DeleteEvents(string eventId);
    }
}