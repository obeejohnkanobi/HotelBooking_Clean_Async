using System.Collections.Generic;
using HotelBooking.Core;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController : Controller
    {
        private IRepository<Booking> bookingRepository;
        private IBookingManager bookingManager;

        public BookingsController(IRepository<Booking> bookingRepos, IBookingManager manager)
        {
            bookingRepository = bookingRepos;
            bookingManager = manager;
        }

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <returns>All bookings.</returns>
        [HttpGet(Name = "GetBookings")]
        public async Task<IEnumerable<Booking>> Get()
        {
            return await bookingRepository.GetAllAsync();
        }

        /// <summary>
        /// Gets booking by identifier.
        /// </summary>
        /// <param name="id">Booking identifier.</param>
        /// <returns>The booking when found; otherwise <see cref="NotFoundResult"/>.</returns>
        [HttpGet("{id}", Name = "GetBooking")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await bookingRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Creates a booking when at least one room is available.
        /// </summary>
        /// <param name="booking">Booking request body.</param>
        /// <returns>201 on success, 400 for invalid input, 409 when no room is available.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Booking booking)
        {
            if (booking == null)
            {
                return BadRequest();
            }

            bool created;
            try
            {
                // Domain validation lives in BookingManager; controller maps
                // domain outcomes/exceptions to protocol-level HTTP responses.
                created = await bookingManager.CreateBooking(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            if (created)
            {
                // 201 + route to created resource keeps API behavior REST-friendly.
                return CreatedAtRoute("GetBooking", new { id = booking.Id }, booking);
            }

            // Conflict communicates that payload shape is valid but state prevents action.
            return Conflict("The booking could not be created. All rooms are occupied. Please try another period.");

        }

        /// <summary>
        /// Updates mutable booking fields.
        /// </summary>
        /// <param name="id">Booking identifier.</param>
        /// <param name="booking">Updated booking payload.</param>
        /// <returns>NoContent on success, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Booking booking)
        {
            if (booking == null || booking.Id != id)
            {
                return BadRequest();
            }

            var modifiedBooking = await bookingRepository.GetAsync(id);

            if (modifiedBooking == null)
            {
                return NotFound();
            }

            // Safety rule: mutating date range or room requires re-running availability
            // checks, so this endpoint only allows state/customer updates.
            modifiedBooking.IsActive = booking.IsActive;
            modifiedBooking.CustomerId = booking.CustomerId;

            await bookingRepository.EditAsync(modifiedBooking);
            return NoContent();
        }

        /// <summary>
        /// Deletes a booking by identifier.
        /// </summary>
        /// <param name="id">Booking identifier.</param>
        /// <returns>NoContent on success, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await bookingRepository.GetAsync(id) == null)
            {
                return NotFound();
            }

            await bookingRepository.RemoveAsync(id);
            return NoContent();
        }

    }
}
