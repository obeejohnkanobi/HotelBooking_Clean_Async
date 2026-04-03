# Annotated Code Excerpts (Professor Mode)

This file provides line-by-line teaching commentary for the most exam-relevant code.

## Excerpt 1 - `BookingManager.FindAvailableRoom`

Source: `HotelBooking.Core/Services/BookingManager.cs`

```csharp
public async Task<int> FindAvailableRoom(DateTime startDate, DateTime endDate)
{
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

        bool roomIsAvailable = activeBookingsForCurrentRoom.All(b =>
            endDate < b.StartDate || startDate > b.EndDate);

        if (roomIsAvailable)
        {
            return room.Id;
        }
    }

    return -1;
}
```

### Line-by-line commentary

- `if (startDate <= DateTime.Today || startDate > endDate)`: fail-fast validation; rejects invalid domain states before data work.
- `GetAllAsync()` calls: pulls required state through abstractions (`IRepository<T>`), preserving testability.
- `bookings.Where(b => b.IsActive)`: inactive bookings are excluded from availability pressure.
- `foreach (var room in rooms)`: first-fit room-selection policy (simple and deterministic).
- `All(...)`: mathematical non-overlap rule; room is available only if request does not overlap **every** active booking for that room.
- `return room.Id`: early return reduces unnecessary iteration after the first valid room.
- `return -1`: explicit sentinel for "no room available".

### Theory note

This method is a compact example of decision logic suitable for:

- Equivalence partitioning (valid/invalid dates),
- Boundary value analysis (`startDate == today`, `startDate > endDate`),
- White-box branch/path design.

---

## Excerpt 2 - `BookingManager.GetFullyOccupiedDates`

Source: `HotelBooking.Core/Services/BookingManager.cs`

```csharp
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

    for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
    {
        int activeBookingsOnDate = activeBookings.Count(b => currentDate >= b.StartDate && currentDate <= b.EndDate);
        if (activeBookingsOnDate >= numberOfRooms)
        {
            fullyOccupiedDates.Add(currentDate);
        }
    }

    return fullyOccupiedDates;
}
```

### Line-by-line commentary

- Guard clause: protects logical consistency (`startDate <= endDate`).
- `numberOfRooms`: transforms capacity into a numeric threshold for occupancy detection.
- `activeBookings`: pre-filter avoids repeated inactive checks in the loop.
- Early return on no bookings/no rooms: avoids meaningless loop execution.
- Date loop: inclusive window (`<= endDate`) to include both boundaries.
- `Count(...) >= numberOfRooms`: occupancy criterion based on supply-demand threshold.

### Theory note

This is a classic counting algorithm over a temporal range (O(days * bookings)).
The implementation optimizes for clarity and testability over advanced data structures.

---

## Excerpt 3 - API Outcome Mapping in `BookingsController.Post`

Source: `HotelBooking.WebApi/Controllers/BookingsController.cs`

```csharp
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
        created = await bookingManager.CreateBooking(booking);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }

    if (created)
    {
        return CreatedAtRoute("GetBooking", new { id = booking.Id }, booking);
    }

    return Conflict("The booking could not be created. All rooms are occupied. Please try another period.");
}
```

### Line-by-line commentary

- Null payload -> `400`: syntactic/shape-level client error.
- `try/catch ArgumentException`: maps domain validation failures into HTTP-level bad request.
- `created == true` -> `201`: resource created with discoverable location.
- `created == false` -> `409`: valid request but current resource state blocks operation.

### Theory note

This is protocol semantics quality: preserving a faithful mapping from domain outcomes to HTTP status classes.

