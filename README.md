# HotelBooking Clean Async - Software Quality Study Edition

This repository is a learning-first implementation inspired by the original HotelBooking sample.
It is intentionally structured to support three quality-focused assignments:

1. Unit testing of business logic (`HotelBooking.Core`)
2. Black-box testing of Create Booking (SpecFlow + Postman)
3. White-box analysis of `FindAvailableRoom` and `GetFullyOccupiedDates`

## 1) Architecture and Separation of Concerns

### What it is
Clean Architecture separates business rules from technical details (database, HTTP, UI).

### Why it exists
If business rules depend on frameworks, tests become slow and fragile. Isolating rules makes them testable and maintainable.

### How it works here
- `HotelBooking.Core`: entities, interfaces, and `BookingManager` business rules only
- `HotelBooking.Infrastructure`: EF Core repositories (`IRepository<T>` implementations)
- `HotelBooking.WebApi`: controllers and dependency injection wiring
- `HotelBooking.UnitTests`: fast isolated tests with Moq
- `HotelBooking.SpecFlowTests`: BDD black-box scenarios with Gherkin

### Trade-offs
- Pros: high testability, low coupling, easier refactoring
- Cons: more abstractions and files to maintain

## 2) Dependency Inversion Principle (DIP)

### What it is
High-level policy should depend on abstractions, not concrete implementations.

### Why it exists
It allows replacing infrastructure with test doubles in unit tests.

### How it works here
`BookingManager` depends on `IRepository<Booking>` and `IRepository<Room>`, not EF Core classes.

### Trade-offs
- Pros: testability and flexibility
- Cons: extra interfaces and wiring code

## 3) Generic Repository Pattern (`IRepository<T>`)

### What it is
A reusable CRUD contract over entities.

### Why it exists
Avoids duplicating the same data-access contract for each entity type.

### How it works here
`BookingRepository`, `RoomRepository`, and `CustomerRepository` implement `IRepository<T>`.

### Trade-offs
- Pros: consistency and easier mocking
- Cons: generic repositories can hide query-specific intent if overused

## 4) Testing Layers

- **Unit test**: tests one unit in isolation (e.g., `BookingManager`) using mocks/fakes
- **Integration test**: tests interaction between components (e.g., real DB provider + repositories)
- **API test**: tests HTTP contracts/status codes/payloads via external clients (Postman)

## 5) AAA Pattern

All tests use Arrange-Act-Assert:
- Arrange test data and dependencies
- Act by calling the method under test
- Assert expected outcome and interactions

## 6) Test Doubles

- **Fake**: simple working implementation (in-memory repository)
- **Stub**: pre-programmed return values
- **Mock**: verifies interactions (`Verify`, call count)
- **Spy**: records calls for later assertions

In this solution:
- Unit tests use **Moq mocks** for strict interaction checks.
- SpecFlow step definitions use **manual in-memory fakes** for readability and black-box flow.

## 7) Data-Driven Testing with xUnit

`[Theory]` + `[InlineData]` are used for boundary checks such as:
- invalid date ranges
- fully occupied date window behavior

This reduces duplication and improves coverage transparency.

## 8) Black-box Design (EP + BVA)

For Create Booking, inputs are partitioned into classes:
- valid range + room available => success
- valid range + no room available => conflict/failure
- invalid date ranges => rejected

Boundary cases include:
- `startDate == today` (invalid)
- `startDate > endDate` (invalid)
- date exactly adjacent to existing bookings

Implemented in:
- `HotelBooking.SpecFlowTests/Features/CreateBooking.feature`
- `HotelBooking.SpecFlowTests/Steps/CreateBookingSteps.cs`

## 9) White-box Coverage Concepts

- **Statement coverage**: each executable line runs at least once
- **Branch coverage**: each decision outcome (`true`/`false`) runs at least once
- **Path coverage**: unique decision paths are executed (usually hardest)

See `docs/white-box-analysis.md` for DD-path and cyclomatic complexity.

## 10) Build and Test

```powershell
dotnet restore .\HotelBooking.sln
dotnet build .\HotelBooking.sln -c Debug
dotnet test .\HotelBooking.UnitTests\HotelBooking.UnitTests.csproj
dotnet test .\HotelBooking.IntegrationTests\HotelBooking.IntegrationTests.csproj
dotnet test .\HotelBooking.SpecFlowTests\HotelBooking.SpecFlowTests.csproj
```

## 11) Postman Automation

Use `Postman/HotelBooking.postman_collection.json` for Create Booking endpoint checks.
Collection includes success and conflict validation scripts.
