using Google.Apis.Calendar.v3.Data;

namespace BD_Assignment.Dtos
{
    public class AddEventDto
    {
        
        public string summary { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public DateTime startDateTime {get; set;}
        public DateTime endDateTime {get; set;}
        
        public EventDateTime getStartDateTime(){
            return new EventDateTime { DateTime = startDateTime };
        }
        public EventDateTime getEndDateTime(){
            return new EventDateTime { DateTime = endDateTime };
        }
    }
}