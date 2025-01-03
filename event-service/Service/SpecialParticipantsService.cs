using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;

namespace event_service.Service
{
    public class SpecialParticipantsService : ISpecialParticipants
    {
        private readonly EventDbContext _context;

        public SpecialParticipantsService(EventDbContext context)
        {
            _context = context;

        }

        public async Task<List<Special_Participants>> GetSpecialParticipantsAsync(int eventId)
        {
            var participants = await _context.Special_Participants
                .Where(p => p.eventId == eventId)
                .OrderBy(p => p.role != "Speaker")
                .ThenBy(p => p.role)
                .ToListAsync();

            return participants;
        }

        public async Task<bool> RemoveSpecialParticipantAsync(int eventId, int participantId)
        {
            try
            {
                var participant = await _context.Special_Participants
                    .FirstOrDefaultAsync(p => p.eventId == eventId && p.id == participantId);

                if (participant == null)
                {
                    return false;
                }

                _context.Special_Participants.Remove(participant);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<bool> AddSpecialParticipantAsync(int eventId, string name, string role, string description, string photoUrl)
        {
            var specialParticipant = new Special_Participants
            {
                eventId = eventId,
                name = name,
                role = role,
                description = description,
                photoUrl = photoUrl,
                registration_Date = DateTime.UtcNow
            };

            try
            {
                _context.Special_Participants.Add(specialParticipant);
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
