# Chapter 3 - Testing Foundations, AAA, and Test Doubles

## Learning goals

- Distinguish unit, integration, and API tests
- Apply AAA structure correctly
- Explain Fake vs Stub vs Mock vs Spy and when to use each

## 1) Unit vs Integration vs API tests

- **Unit test**: isolated method/class, no real DB/web server
- **Integration test**: real component interaction (e.g., EF Core + repositories)
- **API test**: HTTP-level behavior validation (status codes and payloads)

In this project:
- Unit: `HotelBooking.UnitTests`
- Integration: `HotelBooking.IntegrationTests`
- API/Black-box automation: Postman collection in `Postman/HotelBooking.postman_collection.json`

### Science behind test layering

This follows the **test pyramid** principle:

- many unit tests (fast, precise fault localization)
- fewer integration tests (higher realism, slower feedback)
- selective API/E2E tests (highest realism, highest maintenance)

The pyramid minimizes cost per defect found while preserving confidence.

## 2) AAA pattern

### Definition

Arrange, Act, Assert.

### Example from unit tests

```csharp
// Arrange
var sut = new BookingManager(bookingRepository.Object, roomRepository.Object);
var request = new Booking { StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(4) };

// Act
var created = await sut.CreateBooking(request);

// Assert
Assert.False(created);
bookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never());
```

### Why AAA exists

It makes test intent obvious and reduces fragile, mixed-purpose tests.

From an empirical quality perspective, AAA improves **diagnosability**: when a test fails,
you can quickly determine whether setup, behavior, or expectation is wrong.

## 3) Test doubles taxonomy

- **Fake**: lightweight functional implementation
- **Stub**: canned return values
- **Mock**: interaction verification
- **Spy**: records call info for assertions

### Project mapping

- **Fake**: `InMemoryRepository<T>` in `HotelBooking.SpecFlowTests/Steps/CreateBookingSteps.cs`
- **Stub behavior** via Moq: `.Setup(x => x.GetAllAsync()).ReturnsAsync(...)`
- **Mock behavior** via Moq: `.Verify(x => x.AddAsync(...), Times.Once)`
- **Spy-like** (legacy style): fields such as `addWasCalled` in old fake repos

## 4) Manual fakes vs Moq

Use **manual fakes** when scenario readability and evolving state matter (BDD steps).
Use **Moq** when interaction precision and quick setup matter (unit tests).

## 5) Test oracle quality

A test is only as good as its oracle (the assertion logic that decides pass/fail).

- Weak oracle example: `Assert.NotNull(result)` when business rule expects a precise room id.
- Strong oracle example: verify return value and side effects (`AddAsync` called once/never).

Strong oracles reduce **false confidence** (tests pass while behavior is still wrong).

## 6) Oral defense talking points

- "I select test level by feedback speed and confidence target."
- "I use AAA to keep tests readable and audit-friendly."
- "I use Moq for interaction checks and fakes for stateful behavior scenarios."

