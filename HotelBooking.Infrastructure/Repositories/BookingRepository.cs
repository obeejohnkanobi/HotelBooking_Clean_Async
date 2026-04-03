using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRepository{T}"/> for bookings.
    /// </summary>
    public class BookingRepository : IRepository<Booking>
    {
        private readonly HotelBookingContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public BookingRepository(HotelBookingContext context)
        {
            db = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(Booking entity)
        {
            db.Booking.Add(entity);
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EditAsync(Booking entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Booking> GetAsync(int id)
        {
            return await db.Booking.Include(b => b.Customer).
                Include(b => b.Room).
                FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await db.Booking.Include(b => b.Customer).
                Include(b => b.Room).
                ToListAsync();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(int id)
        {
            var booking = await db.Booking.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return;
            }

            db.Booking.Remove(booking);
            await db.SaveChangesAsync();
        }

    }
}
