namespace UrbanSolution.Models.MappingTables
{
    public class EventUser
    {
        public string ParticipantId { get; set; }

        public User Participant { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
