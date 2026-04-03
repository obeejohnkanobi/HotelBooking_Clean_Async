using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Core;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace HotelBooking.SpecFlowTests.Steps
{
    [Binding]
    public sealed class CreateBookingSteps
    {
        private readonly InMemoryRepository<Booking> bookingRepository = new InMemoryRepository<Booking>();
        private readonly InMemoryRepository<Room> roomRepository = new InMemoryRepository<Room>();

        private BookingManager manager = null!;
        private bool? createResult;
        private Exception? capturedException;

        [Given(@"the hotel has (.*) rooms")]
        public void GivenTheHotelHasRooms(int count)
        {
            roomRepository.Seed(Enumerable.Range(1, count).Select(i => new Room { Id = i, Description = $"Room-{i}" }));
            manager = new BookingManager(bookingRepository, roomRepository);
        }

        [Given(@"active bookings from (.*) to (.*) in both rooms")]
        public void GivenActiveBookingsForAllRooms(int startOffset, int endOffset)
        {
            var startDate = DateTime.Today.AddDays(startOffset);
            var endDate = DateTime.Today.AddDays(endOffset);
            var bookings = roomRepository.Items.Select(room => new Booking
            {
                Id = room.Id,
                RoomId = room.Id,
                CustomerId = room.Id,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
            });

            bookingRepository.Seed(bookings);
        }

        [Given("no existing active bookings")]
        public void GivenNoExistingActiveBookings()
        {
            bookingRepository.Seed(Array.Empty<Booking>());
        }

        [When(@"I request a booking from (.*) to (.*)")]
        public async Task WhenIRequestABookingFromTo(int startOffset, int endOffset)
        {
            var request = new Booking
            {
                StartDate = DateTime.Today.AddDays(startOffset),
                EndDate = DateTime.Today.AddDays(endOffset),
                CustomerId = 99,
            };

            try
            {
                createResult = await manager.CreateBooking(request);
                capturedException = null;
            }
            catch (Exception ex)
            {
                capturedException = ex;
                createResult = null;
            }
        }

        [Then(@"booking creation should be (.*)")]
        public void ThenBookingCreationShouldBe(bool expected)
        {
            Assert.That(capturedException, Is.Null);
            Assert.That(createResult, Is.EqualTo(expected));
        }

        [Then("the request should be rejected as invalid")]
        public void ThenTheRequestShouldBeRejectedAsInvalid()
        {
            Assert.That(capturedException, Is.TypeOf<ArgumentException>());
        }

        private sealed class InMemoryRepository<T> : IRepository<T>
        {
            private readonly List<T> items = new List<T>();

            public IEnumerable<T> Items => items;

            public void Seed(IEnumerable<T> source)
            {
                items.Clear();
                items.AddRange(source);
            }

            public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(items.ToList());

            public Task<T> GetAsync(int id)
            {
                var itemWithId = items.FirstOrDefault(entity =>
                {
                    var idProperty = entity!.GetType().GetProperty("Id");
                    return idProperty != null && (int)idProperty.GetValue(entity)! == id;
                });

                return Task.FromResult(itemWithId!);
            }

            public Task AddAsync(T entity)
            {
                items.Add(entity);
                return Task.CompletedTask;
            }

            public Task EditAsync(T entity)
            {
                return Task.CompletedTask;
            }

            public Task RemoveAsync(int id)
            {
                return Task.CompletedTask;
            }
        }
    }
}

