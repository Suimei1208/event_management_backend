using event_service.Model;
using System.ComponentModel.DataAnnotations;

namespace event_service.DTO
{
    public class ParticipantsDto
    {
        public int id { get; set; }
        public string userId { get; set; }
        public int eventId { get; set; }
        public DateTime registration_Date { get; set; }
        public string status { get; set; }
        public string role { get; set; }
    }

    public static class ParticipantsMapper
    {
        public static ScheduleParticipants ToEntity(this ParticipantsDto participantsDto)
        {
            return new ScheduleParticipants
            {
                id = participantsDto.id,
                userId = participantsDto.userId,
                eventId = participantsDto.eventId,
                registration_Date = participantsDto.registration_Date,
                status = participantsDto.status,
                role = participantsDto.role
            };
        }

        public static ParticipantsDto ToDto(this ScheduleParticipants participants)
        {
            return new ParticipantsDto
            {
                id = participants.id,
                userId = participants.userId,
                eventId = participants.eventId,
                registration_Date = participants.registration_Date,
                role = participants.role,
                status = participants.status
            };
        }
    }
}
