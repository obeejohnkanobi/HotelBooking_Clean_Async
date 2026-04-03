# Chapter 2 - Generic Repository and Core Design

## Learning goals

- Understand what `IRepository<T>` is and why it exists
- Understand the two core methods: `FindAvailableRoom` and `GetFullyOccupiedDates`
- Be able to discuss logic correctness and boundaries

## 1) Generic Repository Pattern (`IRepository<T>`)

### Definition

A generic repository is a reusable interface for CRUD-style operations.

```csharp
public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetAsync(int id);
    Task AddAsync(T entity);
    Task EditAsync(T entity);
    Task RemoveAsync(int id);
}
```

### Why it exists

- Avoid duplicate CRUD contracts per entity
- Make service code depend on one consistent abstraction
- Make mocking easy in tests

### Trade-offs

- Can become too generic for complex queries
- Works best when paired with clear domain services (`BookingManager`)

## 2) `FindAvailableRoom` business rule

### Problem solved

Given start/end dates, find the first room with no overlap against active bookings.

### Key logic

```csharp
bool roomIsAvailable = activeBookingsForCurrentRoom.All(b =>
    endDate < b.StartDate || startDate > b.EndDate);
```

If all bookings for that room are either fully before or fully after requested range, room is available.

### Guard clause

```csharp
if (startDate <= DateTime.Today || startDate > endDate)
{
    throw new ArgumentException("The start date cannot be in the past or later than the end date.");
}
```

### Trade-off

Returns first matching room for speed/simple behavior, not "best room" by price or quality.

## 3) `GetFullyOccupiedDates` business rule

### Problem solved

Within a date range, detect each day where active bookings cover all rooms.

### Key logic

```csharp
int activeBookingsOnDate = activeBookings.Count(b =>
    currentDate >= b.StartDate && currentDate <= b.EndDate);

if (activeBookingsOnDate >= numberOfRooms)
{
    fullyOccupiedDates.Add(currentDate);
}
```

### Guard clause

```csharp
if (startDate > endDate)
{
    throw new ArgumentException("The start date cannot be later than the end date.");
}
```

### Trade-off

Simple O(days * bookings) approach is easy to reason about and test; not optimized for massive data volumes.

## 4) Oral defense talking points

- "I used guard clauses to fail fast on invalid ranges."
- "Overlap logic is explicit and testable at boundary values."
- "The method signatures are async to align with async repository contracts."

