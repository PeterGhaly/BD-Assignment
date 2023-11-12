using BD_Assignment.Dtos;
using BD_Assignment.Models;
using BD_Assignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace BD_Assignment.Controllers
{

    [ApiController]
    [Route("api/events")]


    public class CalenderController : ControllerBase
    {

        private readonly IEventSevice _eventService;
        public CalenderController(IEventSevice eventSevice)
        {
            _eventService = eventSevice;
        }

        [HttpGet]
        public async Task<ActionResult> GetEvents(DateTime? startDate = null, DateTime? endDate = null, string searchQuery = null)
        {
            try
            {
                return Ok(_eventService.GetEvents(startDate,endDate,searchQuery));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving events: {ex.Message}");
            }
        }

        [HttpDelete("{eventId}")]
        public async Task<ActionResult> DeleteEvent(string eventId)
        {
            // Validate the eventId (add your own validation logic as needed)
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("eventId is required.");
            }
            try
            {
                await _eventService.DeleteEvents(eventId);

                // Return a 204 No Content HTTP status code upon successful deletion
                return NoContent();
            }
            catch (Google.GoogleApiException ex)
            {
                // Handle errors such as the event not being found or issues with the API call
                if (ex.Error.Code == 404)
                {
                    return NotFound("Event not found.");
                }
                // Log or handle other specific Google API errors if needed

                // Return a generic error message for other errors
                return StatusCode(500, "Error deleting event.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Error deleting event: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<CalenderEvent>>> AddEvent(AddEventDto newEvent)
        {
            try
            {
                return StatusCode(201, _eventService.AddEvent(newEvent));
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, $"Error creating event: {ex.Message}");
            }
        }

    }
}
public class FixedPortLocalServerCodeReceiver : LocalServerCodeReceiver
{
    private readonly int fixedPort;

    public FixedPortLocalServerCodeReceiver(int fixedPort)
    {
        this.fixedPort = fixedPort;
    }

    public string GetRedirectUri()
    {
        return $"http://localhost:{fixedPort}/authorize/";
    }
}
