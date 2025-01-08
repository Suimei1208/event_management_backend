using event_service.DTO;
using event_service.Interface;
using event_service.Model;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace event_service.Service
{
    public class EventAttendanceService : IEventAttendanceService
    {
        private readonly EventDbContext _context;

        public EventAttendanceService(EventDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventAttendanceDto>> GetCheckedInAndCheckedOutParticipantsAsync(int eventId)
        {
            try
            {
                var participants = await _context.EventAttendances
                    .Where(a => a.eventId == eventId && a.checkIn && a.checkOut)
                    .ToListAsync();

                var result = participants.Select(a => new EventAttendanceDto
                {
                    id = a.id,
                    userId = a.userId,
                    eventId = a.eventId,
                    checkIn = a.checkIn,
                    checkInTime = a.checkInTime,
                    checkOut = a.checkOut,
                    checkOutTime = a.checkOutTime
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching participants: {ex.Message}", ex);
            }
        }


        public async Task RecordCheckInAsync(string qrCode)
        {
            (string userId, int eventId) = DecodeQRCode(qrCode);

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.id == eventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found.");
            }

            var existingAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(a => a.eventId == eventId && a.userId == userId);

            if (existingAttendance != null)
            {
                if (existingAttendance.checkIn)
                {
                    existingAttendance.checkIn = true;
                    existingAttendance.checkInTime = DateTime.UtcNow.AddHours(7);
                    existingAttendance.checkOut = false;
                    existingAttendance.checkOutTime = DateTime.UtcNow.AddHours(7);
                }
                else
                {
                    throw new Exception("User already checked in for this event.");
                }
            }
            else
            {
                // No attendance found, create a new record
                var attendance = new EventAttendance
                {
                    userId = userId,
                    eventId = eventId,
                    checkInTime = DateTime.UtcNow.AddHours(7),
                    checkOutTime = DateTime.UtcNow.AddHours(7),
                    checkIn = true,
                    checkOut = false
                };

                _context.EventAttendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RecordCheckOutAsync(string qrCode)
        {
            (string userId, int eventId) = DecodeQRCode(qrCode);

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.id == eventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found.");
            }

            // Find existing attendance
            var existingAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(a => a.eventId == eventId && a.userId == userId);

            if (existingAttendance == null || !existingAttendance.checkIn)
            {
                throw new Exception("User has not checked in yet.");
            }

            if (!existingAttendance.checkOut) // Check if the user has not checked out yet
            {
                existingAttendance.checkOut = true;
                existingAttendance.checkOutTime = DateTime.UtcNow.AddHours(7);
            }
            else
            {
                throw new Exception("User has already checked out.");
            }

            await _context.SaveChangesAsync();
        }

        private (string userId, int eventId) DecodeQRCode(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                throw new ArgumentException("QR code cannot be null or empty.");
            }

            try
            {
                // Split the QR code into the parts (eventId, userId, timestamp)
                string[] qrParts = qrCode.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (qrParts.Length < 2)
                {
                    throw new FormatException("QR code is improperly formatted.");
                }

                string base64EventId = qrParts[0];
                string base64UserId = qrParts[1];

                int eventId = int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(base64EventId)));
                string userId = Encoding.UTF8.GetString(Convert.FromBase64String(base64UserId));

                return (userId, eventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding QR Code: {ex.Message}");
                throw new Exception($"Error decoding QR code. Details: {ex.Message}", ex);
            }
        }
    }
}
