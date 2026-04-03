using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Core
{
    /// <summary>
    /// Implements booking use cases with pure business rules.
    /// </summary>
    public class BookingManager : IBookingManager
    {
        private readonly IRepository<Booking> bookingRepository;
        private readonly IRepository<Room> roomRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingManager"/> class.
        /// </summary>
        /// <param name="bookingRepository">Booking repository abstraction.</param>
        /// <param name="roomRepository">Room repository abstraction.</param>
        public BookingManager(IRepository<Booking> bookingRepository, IRepository<Room> roomRepository)
        {
            this.bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            this.roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        }

        /// <inheritdoc />
        public async Task<bool> CreateBooking(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }

            // Quality invariant: booking creation must go through the same availability
            // rule used elsewhere so the system has one source of truth.
            int roomId = await FindAvailableRoom(booking.StartDate, booking.EndDate);

            if (roomId < 0)
            {
                return false;
            }

            booking.RoomId = roomId;
            booking.IsActive = true;
            await bookingRepository.AddAsync(booking);
            return true;
        }

        /// <inheritdoc />
        public async Task<int> FindAvailableRoom(DateTime startDate, DateTime endDate)
        {
            // Fail-fast guard: reject invalid ranges before hitting repositories.
            // This keeps invalid-input behavior deterministic and cheap to test.
            if (startDate <= DateTime.Today || startDate > endDate)
            {
                throw new ArgumentException("The start date cannot be in the past or later than the end date.");
            }

            var bookings = await bookingRepository.GetAllAsync();
            var activeBookings = bookings.Where(b => b.IsActive);
            var rooms = await roomRepository.GetAllAsync();

            foreach (var room in rooms)
            {
                var activeBookingsForCurrentRoom = activeBookings.Where(b => b.RoomId == room.Id);

                // Interval overlap science:
                // two ranges overlap unless one is strictly before the other.
                // Non-overlap condition: [requestEnd < existingStart] OR [requestStart > existingEnd].
                // We require non-overlap for ALL active bookings in this room.
                bool roomIsAvailable = activeBookingsForCurrentRoom.All(b =>
                    endDate < b.StartDate || startDate > b.EndDate);

                if (roomIsAvailable)
                {
                    return room.Id;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        public async Task<List<DateTime>> GetFullyOccupiedDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("The start date cannot be later than the end date.");
            }

            var fullyOccupiedDates = new List<DateTime>();
            var rooms = await roomRepository.GetAllAsync();
            int numberOfRooms = rooms.Count();
            var bookings = await bookingRepository.GetAllAsync();
            var activeBookings = bookings.Where(b => b.IsActive).ToList();

            if (!activeBookings.Any() || numberOfRooms == 0)
            {
                return fullyOccupiedDates;
            }

            // Day-by-day scan is intentionally explicit for reasoning and testability.
            // It favors clarity over micro-optimizations because this method is mainly
            // used as a reporting/query rule, not a high-frequency write path.
            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                int activeBookingsOnDate = activeBookings.Count(b => currentDate >= b.StartDate && currentDate <= b.EndDate);
                // Fully occupied means demand at least equals capacity.
                if (activeBookingsOnDate >= numberOfRooms)
                {
                    fullyOccupiedDates.Add(currentDate);
                }
            }

            return fullyOccupiedDates;
        }

    }
}
