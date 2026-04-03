using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(2, 1)]
        public async Task FindAvailableRoom_InvalidDateRange_ThrowsArgumentException(int startOffset, int endOffset)
        {
            // Arrange
            var sut = CreateManager(new List<Booking>(), new List<Room> { new Room { Id = 1, Description = "A" } });
            var startDate = DateTime.Today.AddDays(startOffset);
            var endDate = DateTime.Today.AddDays(endOffset);

            // Act
            Task<int> action() => sut.FindAvailableRoom(startDate, endDate);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task FindAvailableRoom_WithOverlappingActiveBookings_ReturnsMinusOne()
        {
            // Arrange
            var requestStart = DateTime.Today.AddDays(5);
            var requestEnd = DateTime.Today.AddDays(7);
            var bookings = new List<Booking>
            {
                new Booking { RoomId = 1, StartDate = DateTime.Today.AddDays(4), EndDate = DateTime.Today.AddDays(8), IsActive = true },
                new Booking { RoomId = 2, StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(9), IsActive = true },
            };

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
            };

            var sut = CreateManager(bookings, rooms);

            // Act
            var roomId = await sut.FindAvailableRoom(requestStart, requestEnd);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_WithBoundaryGap_ReturnsFirstAvailableRoom()
        {
            // Arrange
            var requestStart = DateTime.Today.AddDays(10);
            var requestEnd = DateTime.Today.AddDays(12);
            var bookings = new List<Booking>
            {
                new Booking { RoomId = 1, StartDate = DateTime.Today.AddDays(8), EndDate = DateTime.Today.AddDays(9), IsActive = true },
                new Booking { RoomId = 2, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(15), IsActive = true },
            };

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
            };

            var sut = CreateManager(bookings, rooms);

            // Act
            var roomId = await sut.FindAvailableRoom(requestStart, requestEnd);

            // Assert
            Assert.Equal(1, roomId);
        }

        [Fact]
        public async Task CreateBooking_WhenRoomExists_SetsRoomAndIsActiveAndPersists()
        {
            // Arrange
            var bookingRepository = new Mock<IRepository<Booking>>();
            var roomRepository = new Mock<IRepository<Room>>();

            roomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room>
            {
                new Room { Id = 3, Description = "Suite" }
            });

            bookingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Booking>());

            Booking persisted = null;
            bookingRepository
                .Setup(r => r.AddAsync(It.IsAny<Booking>()))
                .Callback<Booking>(b => persisted = b)
                .Returns(Task.CompletedTask);

            var sut = new BookingManager(bookingRepository.Object, roomRepository.Object);
            var newBooking = new Booking
            {
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(3),
                CustomerId = 42,
            };

            // Act
            var created = await sut.CreateBooking(newBooking);

            // Assert
            Assert.True(created);
            Assert.NotNull(persisted);
            Assert.Equal(3, persisted.RoomId);
            Assert.True(persisted.IsActive);
            bookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CreateBooking_WhenNoRoomAvailable_ReturnsFalseAndDoesNotPersist()
        {
            // Arrange
            var bookingRepository = new Mock<IRepository<Booking>>();
            var roomRepository = new Mock<IRepository<Room>>();

            roomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room>
            {
                new Room { Id = 1, Description = "A" }
            });

            bookingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Booking>
            {
                new Booking
                {
                    RoomId = 1,
                    StartDate = DateTime.Today.AddDays(2),
                    EndDate = DateTime.Today.AddDays(6),
                    IsActive = true,
                }
            });

            var sut = new BookingManager(bookingRepository.Object, roomRepository.Object);
            var request = new Booking
            {
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(4),
                CustomerId = 10,
            };

            // Act
            var created = await sut.CreateBooking(request);

            // Assert
            Assert.False(created);
            bookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never());
        }

        [Theory]
        [InlineData(0, 2, true)]
        [InlineData(1, 3, true)]
        [InlineData(2, 4, false)]
        public async Task GetFullyOccupiedDates_BoundaryCases_ReturnsExpectedCoverage(int startOffset, int endOffset, bool expectsDayTwo)
        {
            // Arrange
            var baseDate = DateTime.Today.AddDays(10);
            var bookings = new List<Booking>
            {
                new Booking { RoomId = 1, StartDate = baseDate, EndDate = baseDate.AddDays(2), IsActive = true },
                new Booking { RoomId = 2, StartDate = baseDate.AddDays(1), EndDate = baseDate.AddDays(3), IsActive = true },
            };

            var rooms = new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
            };

            var sut = CreateManager(bookings, rooms);
            var startDate = baseDate.AddDays(startOffset);
            var endDate = baseDate.AddDays(endOffset);

            // Act
            var occupiedDates = await sut.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectsDayTwo, occupiedDates.Contains(baseDate.AddDays(1)));
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartGreaterThanEnd_ThrowsArgumentException()
        {
            // Arrange
            var sut = CreateManager(new List<Booking>(), new List<Room> { new Room { Id = 1, Description = "A" } });
            var start = DateTime.Today.AddDays(6);
            var end = DateTime.Today.AddDays(4);

            // Act
            Task<List<DateTime>> action() => sut.GetFullyOccupiedDates(start, end);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        private static BookingManager CreateManager(List<Booking> bookings, List<Room> rooms)
        {
            var bookingRepository = new Mock<IRepository<Booking>>();
            var roomRepository = new Mock<IRepository<Room>>();

            bookingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);
            bookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
            roomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);

            return new BookingManager(bookingRepository.Object, roomRepository.Object);
        }
    }
}
