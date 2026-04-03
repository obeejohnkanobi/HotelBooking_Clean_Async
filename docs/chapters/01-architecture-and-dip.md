# Chapter 1 - Clean Architecture and Dependency Inversion

## Learning goals

- Understand separation of concerns in this solution
- Explain why `HotelBooking.Core` has zero infrastructure/framework dependencies
- Explain how DIP enables testability

## 1) What is Clean Architecture?

Clean Architecture is a way to organize code so business rules are independent of tools like web frameworks and databases.

### Why it exists

Without boundaries, business logic gets mixed with HTTP/EF code. This makes logic hard to test and expensive to change.

### How it works in this project

- `HotelBooking.Core`: business rules (`BookingManager`, entities, contracts)
- `HotelBooking.Infrastructure`: EF Core repositories and database setup
- `HotelBooking.WebApi`: delivery layer (HTTP controllers, DI wiring)

### Concrete example

`BookingManager` depends on abstractions, not EF Core types:

```csharp
public class BookingManager : IBookingManager
{
    private readonly IRepository<Booking> bookingRepository;
    private readonly IRepository<Room> roomRepository;

    public BookingManager(IRepository<Booking> bookingRepository, IRepository<Room> roomRepository)
    {
        this.bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        this.roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    }
}
```

### Trade-offs

- Upside: high testability, low coupling, safer refactoring
- Downside: more interfaces, more DI wiring, more files

### Science behind the decision

- **Coupling theory**: tighter coupling increases defect propagation because a local change has more hidden side effects.
- **Cohesion theory**: modules with one clear reason to change are easier to test and reason about.
- **Change amplification**: poor boundaries force one feature change to touch many files/layers.

Clean Architecture reduces coupling and change amplification by keeping domain policy independent.

## 2) What is Dependency Inversion Principle (DIP)?

High-level policy should depend on abstractions, not concrete details.

### Why it exists

If business logic directly creates EF contexts or controllers, unit tests need infrastructure and become slow.

### How it works here

`Program.cs` wires concrete repositories at runtime, but `Core` never sees these implementations.

```csharp
builder.Services.AddScoped<IRepository<Room>, RoomRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Booking>, BookingRepository>();
builder.Services.AddScoped<IBookingManager, BookingManager>();
```

### Trade-offs and alternatives

- Alternative: direct concrete dependencies (fewer files, poor testability)
- DIP choice: more abstraction cost, much better quality and flexibility

### Reliability perspective

From a reliability engineering angle, DIP improves **fault isolation**:

- If a data-access adapter fails, business rules can still be tested independently.
- If a business rule changes, infrastructure adapters usually remain unchanged.

This separation lowers the probability that one defect class contaminates unrelated components.

## 3) Oral defense talking points

- "`Core` is policy; `Infrastructure` is mechanism."
- "We inject `IRepository<T>` so tests can replace DB with Moq or in-memory fakes."
- "This design supports unit, integration, and API testing at different speeds and costs."

