using System.Globalization;
using Google.Apis.Calendar.v3.Data;

namespace BD_Assignment.Models
{
    public class CalenderEvent
    {
        public CalenderEvent(Event _event)
        {
            Id = _event.Id;
            summary = _event.Summary;
            location = _event.Location;
            description = _event.Description;
            startEventDateTime = _event.Start;
            endEventDateTime = _event.End;
            // Convert DateTime to DateTimeOffset
                    DateTimeOffset startDateTimeOffset = DateTimeOffset.ParseExact(
                        startEventDateTime.DateTimeRaw,
                        "yyyy-MM-ddTHH:mm:sszzz", // Adjust the format as needed
                        CultureInfo.InvariantCulture
                    );

                    DateTimeOffset endDateTimeOffset = DateTimeOffset.ParseExact(
                        endEventDateTime.DateTimeRaw,
                        "yyyy-MM-ddTHH:mm:sszzz", // Adjust the format as needed
                        CultureInfo.InvariantCulture
                    );
                    startDateTime = startDateTimeOffset.DateTime;
                    endDateTime = endDateTimeOffset.DateTime;
        }
        public string Id { get; set; }
        public string summary { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        private EventDateTime startEventDateTime { get;}
        private EventDateTime endEventDateTime { get;}

        //Date Times that can be printed
        public DateTime startDateTime {get;}
        public DateTime endDateTime {get;}
    }
}