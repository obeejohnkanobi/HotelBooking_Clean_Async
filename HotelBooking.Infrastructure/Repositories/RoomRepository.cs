using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRepository{T}"/> for rooms.
    /// </summary>
    public class RoomRepository : IRepository<Room>
    {
        private readonly HotelBookingContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public RoomRepository(HotelBookingContext context)
        {
            db = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(Room entity)
        {
            db.Room.Add(entity);
            
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EditAsync(Room entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Room> GetAsync(int id)
        {
            // The FirstOrDefault method below returns null
            // if there is no room with the specified Id.
            return await db.Room.FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await db.Room.ToListAsync();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(int id)
        {
            var room = await db.Room.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return;
            }

            db.Room.Remove(room);
            await db.SaveChangesAsync();
        }
    }
}
