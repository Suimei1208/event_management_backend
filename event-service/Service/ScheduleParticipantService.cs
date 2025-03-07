using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class ScheduleParticipantService : IScheduleParticipants
    {
        private readonly EventDbContext _context;
        public ScheduleParticipantService(EventDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule_Participants>> GetScheduleParticipantsAsync(int eventId)
        {
            var participants = await _context.Schedule_Participants
                .Where(p => p.scheduleId == eventId)
                .ToListAsync();

            return participants;
        }

        public async Task<bool> RemoveScheduleParticipantAsync(int eventId, string participantId)
        {
            try
            {
                var participant = await _context.Schedule_Participants
                    .FirstOrDefaultAsync(p => p.scheduleId == eventId && p.userId == participantId);

                if (participant == null)
                {
                    return false;
                }

                _context.Schedule_Participants.Remove(participant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }   

}
