using Confluent.Kafka;
using event_service.DTO;
using event_service.Interface;
using event_service.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using user_services.DTO;

namespace event_service.Service
{
    public class EventAttendanceService : IEventAttendanceService
    {
        private readonly EventDbContext _context;
        private static IHttpContextAccessor _httpContextAccessor;
        private static readonly HttpClient client = new HttpClient();

        public EventAttendanceService(EventDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private static async Task<CustomUser> GetCustomUserAsync(string uid)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var token = httpContext.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync($"http://user-services:5000/api/Users/GetUserById?userId={uid}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);
                if (responseData.ContainsKey("success") && responseData["success"] == true)
                {
                    //Console.WriteLine("User Data: " + responseData["data"]);

                    var user = new CustomUser()
                    {
                        id = responseData["data"]["id"],
                        NameFromEmail = responseData["data"]["nameFromEmail"],
                    };

                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
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

        public async Task<List<EventAttendanceDto>> GetCheckedInParticipantsAsync(int eventId)
        {
            try
            {
                var participants = await _context.EventAttendances
                    .Where(a => a.eventId == eventId && a.checkIn)
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

        public async Task<List<EventAttendanceDto>> GetCheckedOutParticipantsAsync(int eventId)
        {
            try
            {
                var participants = await _context.EventAttendances
                    .Where(a => a.eventId == eventId && a.checkOut)
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

        public async Task RecordCheckInManuallyAsync(int eventId, string inputName)
        {

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.id == eventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found.");
            }

            var participants = await _context.Participants
        .Where(p => p.eventId == eventId)
        .ToListAsync();

            // Find the userId corresponding to the inputName
            string? matchedUserId = null;

            foreach (var participant in participants)
            {
                // Get user details using the external service
                var user = await GetCustomUserAsync(participant.userId);
                if (user != null && user.NameFromEmail == inputName)
                {
                    matchedUserId = user.id;
                    break;
                }
            }

            if (matchedUserId == null)
            {
                throw new Exception("No participant found with the provided name for this event.");
            }

            var existingAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(a => a.eventId == eventId && a.userId == matchedUserId);

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
                    userId = matchedUserId,
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

        public async Task RecordCheckOutManuallyAsync(int eventId, string inputName)
        {

            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.id == eventId);
            if (eventEntity == null)
            {
                throw new Exception("Event not found.");
            }

            var participants = await _context.Participants
        .Where(p => p.eventId == eventId)
        .ToListAsync();

            // Find the userId corresponding to the inputName
            string? matchedUserId = null;

            foreach (var participant in participants)
            {
                // Get user details using the external service
                var user = await GetCustomUserAsync(participant.userId);
                if (user != null && user.NameFromEmail == inputName)
                {
                    matchedUserId = user.id;
                    break;
                }
            }

            if (matchedUserId == null)
            {
                throw new Exception("No participant found with the provided name for this event.");
            }

            // Find existing attendance
            var existingAttendance = await _context.EventAttendances
                .FirstOrDefaultAsync(a => a.eventId == eventId && a.userId == matchedUserId);

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

        public async Task<EventStatisticsDto> GetEventStatisticsAsync(int eventId)
        {
            try
            {
                var existedParticipant = _context.Participants.Where(a => a.eventId == eventId);
                var participants = _context.EventAttendances.Where(a => a.eventId == eventId).ToList();
                int totalParticipants = existedParticipant.Count();
                int checkedInParticipants = participants.Count(p => p.checkIn && p.checkOut);

                if (totalParticipants == 0)
                {
                    return new EventStatisticsDto
                    {
                        AverageParticipationTime = 0,
                        ParticipationPercentage = 0
                    };
                }

                double totalParticipationTimeMinutes = participants
                    .Where(p => p.checkIn && p.checkOut)
                    .Sum(p => (p.checkOutTime - p.checkInTime).TotalMinutes);

                string FormatTime(double totalMinutes)
                {
                    int days = (int)(totalMinutes / (24 * 60));
                    totalMinutes %= (24 * 60);
                    int hours = (int)(totalMinutes / 60);
                    totalMinutes %= 60;
                    int minutes = (int)totalMinutes;
                    double seconds = (totalMinutes - minutes) * 60;

                    if (days > 0)
                        return $"{days} day(s), {hours} hour(s)";
                    if (hours > 0)
                        return $"{hours} hour(s), {minutes} minute(s)";
                    if (minutes > 0)
                        return $"{minutes} minute(s), {seconds:F0} second(s)";
                    return $"{seconds:F0} second(s)";
                }

                double averageParticipationTimeMinutes = totalParticipationTimeMinutes / checkedInParticipants;
                string averageParticipationTimeFormatted = FormatTime(averageParticipationTimeMinutes);

                double participationPercentage = ((double)checkedInParticipants / totalParticipants) * 100;

                return new EventStatisticsDto
                {
                    checkedInParticipants = checkedInParticipants,
                    AverageParticipationTime = averageParticipationTimeMinutes, 
                    AverageParticipationTimeFormatted = averageParticipationTimeFormatted,
                    ParticipationPercentage = participationPercentage
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
