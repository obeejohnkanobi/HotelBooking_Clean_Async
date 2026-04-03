using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Core
{
    /// <summary>
    /// Coordinates hotel booking business rules.
    /// </summary>
    public interface IBookingManager
    {
        /// <summary>
        /// Creates a booking if at least one room is available for the requested period.
        /// </summary>
        /// <param name="booking">Booking request to process.</param>
        /// <returns><c>true</c> when a booking is created; otherwise <c>false</c>.</returns>
        Task<bool> CreateBooking(Booking booking);

        /// <summary>
        /// Finds the first available room for a date range.
        /// </summary>
        /// <param name="startDate">Requested start date.</param>
        /// <param name="endDate">Requested end date.</param>
        /// <returns>Available room identifier, or -1 when no room is available.</returns>
        Task<int> FindAvailableRoom(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all dates where active bookings occupy every room.
        /// </summary>
        /// <param name="startDate">Range start date (inclusive).</param>
        /// <param name="endDate">Range end date (inclusive).</param>
        /// <returns>Fully occupied dates in the requested range.</returns>
        Task<List<DateTime>> GetFullyOccupiedDates(DateTime startDate, DateTime endDate);
    }
}
