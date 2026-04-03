using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRepository{T}"/> for customers.
    /// </summary>
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly HotelBookingContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public CustomerRepository(HotelBookingContext context)
        {
            db = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(Customer entity)
        {
            db.Customer.Add(entity);
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EditAsync(Customer entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<Customer> GetAsync(int id)
        {
            return await db.Customer.FirstOrDefaultAsync(r => r.Id == id);

        }

        /// <inheritdoc />
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await db.Customer.ToListAsync();
        }

        /// <inheritdoc />
        public async Task RemoveAsync(int id)
        {
            var customer = await db.Customer.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return;
            }

            db.Customer.Remove(customer);
            await db.SaveChangesAsync();
        }
    }
}
