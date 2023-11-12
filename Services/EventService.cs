using BD_Assignment.Dtos;
using BD_Assignment.Models;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;


namespace BD_Assignment.Services
{
    public class EventService : IEventSevice
    {
        private readonly CalendarService _calendarService;
        public EventService()
        {
            _calendarService = GetCalendarService();
        }

        public ServiceResponse<CalenderEvent> AddEvent(AddEventDto eventDto)
        {
            // Check if the event's start time is in the past
            if (eventDto.startDateTime < DateTime.Now)
            {
                throw new Exception("Cannot create events in the past.");
            }

            // Check if the event is on Friday or Saturday
            if (eventDto.startDateTime.DayOfWeek == DayOfWeek.Friday || eventDto.startDateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new Exception("Cannot create events on Fridays or Saturdays.");
            }

            // Create a new event based on user input
            Event newEvent = new Event()
            {
                Summary = eventDto.summary,
                Location = eventDto.location,
                Description = eventDto.description,
                Start = eventDto.getStartDateTime(),
                End = eventDto.getEndDateTime(),
            };

            // Specify the calendar ID where you want to add the event (primary for the user's main calendar)
            string calendarId = "primary";

            // Make the API request to create the event
            EventsResource.InsertRequest request = _calendarService.Events.Insert(newEvent, calendarId);
            Event createdEvent = request.Execute();

            ServiceResponse<CalenderEvent> serviceResponse = new ServiceResponse<CalenderEvent>();
            serviceResponse.Data = new CalenderEvent(createdEvent);
            serviceResponse.Success = true;
            serviceResponse.Message = "Created the Event";
            return serviceResponse;
        }


        async Task IEventSevice.DeleteEvents(string eventId)
        {
            // Delete the event using the Google Calendar API
            _calendarService.Events.Delete("primary", eventId).Execute();
        }

        ServiceResponse<List<CalenderEvent>> IEventSevice.GetEvents(DateTime? startDate = null, DateTime? endDate = null, string searchQuery = null)
        {
            // Create the request for events
            EventsResource.ListRequest request = _calendarService.Events.List("primary");
            request.TimeMin = startDate;
            request.TimeMax = endDate;
            request.Q = searchQuery;

            // Execute the request and get the events
            Events events = request.Execute();

            // Extract the list of events from the response
            List<CalenderEvent> eventList = new List<CalenderEvent>();
            foreach (var googleEvent in events.Items)
            {
                // Create a new Event with DateTimeOffset properties
                var newEvent = new CalenderEvent(googleEvent);

                /*{
                    Summary = googleEvent.Summary,
                    Location = googleEvent.Location,
                    Description = googleEvent.Description,
                    Start = new EventDateTime { DateTime = startDateTimeOffset.DateTime, TimeZone = startDateTimeOffset.Offset.ToString() },
                    End = new EventDateTime { DateTime = endDateTimeOffset.DateTime, TimeZone = endDateTimeOffset.Offset.ToString() },
                    Id = googleEvent.Id
                };*/

                eventList.Add(newEvent);
            }

            ServiceResponse<List<CalenderEvent>> serviceResponse = new ServiceResponse<List<CalenderEvent>>();
            serviceResponse.Data = eventList;
            serviceResponse.Message = "Found the List";
            serviceResponse.Success = true;
            return serviceResponse;
        }

        private CalendarService GetCalendarService()
        {
            // Set up the Calendar service using OAuth 2.0 credentials
            // This should include the path to your credentials JSON file
            UserCredential credential;
            using (var stream = new FileStream("Credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "Credentials"; // store user tokens

                var clientSecrets = GoogleClientSecrets.Load(stream).Secrets;

                var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = new[] { CalendarService.Scope.Calendar },
                    DataStore = new FileDataStore(credPath, true),
                });

                var codeReceiver = new LocalServerCodeReceiver(); // Specify your fixed port here
                var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

                credential = authCode.AuthorizeAsync("user", CancellationToken.None).Result;
            }


            // Check if the access token is expired, and refresh if necessary
            if (credential.Token.IsExpired(credential.Flow.Clock))
            {
                bool success = credential.RefreshTokenAsync(CancellationToken.None).Result;
                if (!success)
                {
                    // Handle the case where refresh fails (e.g., user revoked access)
                    Console.WriteLine("Failed to refresh the access token. The user may have revoked access.");
                    Environment.Exit(1);
                }
            }

            // Create the Calendar service
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Web client 2"
            });

            return service;
        }
    }

}