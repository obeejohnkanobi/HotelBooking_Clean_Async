# Professor Commentary - Full Code Walkthrough

This document provides a teaching-style commentary for the codebase, file by file.
For each file: what it does, why it matters for quality, and what theory it demonstrates.

## How to read this

- **Purpose**: role of the file
- **Quality/theory**: software quality principle demonstrated
- **Professor note**: practical critique and oral-defense angle

## Companion study resources

- Annotated code (line-by-line): `docs/annotated-code-excerpts.md`
- Common mistakes and recovery patterns: `docs/common-student-mistakes.md`
- Viva practice questions and model answers: `docs/viva-question-bank.md`

---

## `HotelBooking.Core`

### `HotelBooking.Core/HotelBooking.Core.csproj`
- **Purpose**: core domain project configuration.
- **Quality/theory**: architecture boundary enforcer; `Core` should stay framework-light.
- **Professor note**: targeting `netstandard2.1` maximizes compatibility but can limit modern API usage.

### `HotelBooking.Core/Entities/Booking.cs`
- **Purpose**: aggregate data for reservation state.
- **Quality/theory**: domain model clarity; state variables (`IsActive`, dates, room/customer linkage).
- **Professor note**: model is an anemic entity by design; behavior is intentionally placed in `BookingManager`.

### `HotelBooking.Core/Entities/Customer.cs`
- **Purpose**: customer identity and contact data.
- **Quality/theory**: single-responsibility data model.
- **Professor note**: validation attributes are absent; validation is currently layered elsewhere.

### `HotelBooking.Core/Entities/Room.cs`
- **Purpose**: room identity and description.
- **Quality/theory**: minimal domain surface supports easier testing.
- **Professor note**: no capacity/type fields yet; easy extension point for richer booking policy.

### `HotelBooking.Core/Interfaces/IRepository.cs`
- **Purpose**: async CRUD abstraction for persistence access.
- **Quality/theory**: Dependency Inversion Principle (DIP), test seams.
- **Professor note**: generic repository reduces duplication; complex queries may eventually need dedicated query services.

### `HotelBooking.Core/Interfaces/IBookingManager.cs`
- **Purpose**: business-use-case contract.
- **Quality/theory**: explicit policy interface enables mocking and test layering.
- **Professor note**: public contract expresses core rules clearly (`CreateBooking`, availability, occupancy).

### `HotelBooking.Core/Services/BookingManager.cs`
- **Purpose**: business rule engine for booking lifecycle decisions.
- **Quality/theory**:
  - fail-fast validation (guard clauses),
  - interval overlap logic,
  - capacity-based occupancy analysis,
  - cyclomatic complexity suitable for white-box analysis.
- **Professor note**:
  - strongest quality file in the solution,
  - comments now explain interval algebra and policy invariants,
  - ideal candidate for boundary and path-based tests (already present).

---

## `HotelBooking.Infrastructure`

### `HotelBooking.Infrastructure/HotelBooking.Infrastructure.csproj`
- **Purpose**: data-access adapter project.
- **Quality/theory**: clean architecture outer layer; depends on `Core`.
- **Professor note**: good separation; old-style reference hints should remain removed/unused over time.

### `HotelBooking.Infrastructure/HotelBookingContext.cs`
- **Purpose**: EF Core DbContext for bookings, rooms, customers.
- **Quality/theory**: ORM mapping boundary between domain and storage.
- **Professor note**: simple and readable; good for integration-test setup.

### `HotelBooking.Infrastructure/IDbInitializer.cs`
- **Purpose**: abstraction for database seeding lifecycle.
- **Quality/theory**: startup concern isolated behind interface.
- **Professor note**: supports deterministic demo/testing data.

### `HotelBooking.Infrastructure/DbInitializer.cs`
- **Purpose**: creates and seeds sample data.
- **Quality/theory**: reproducibility, deterministic environment setup.
- **Professor note**: convenient for demos; in production, migrations/seed strategy should be environment-aware.

### `HotelBooking.Infrastructure/Repositories/BookingRepository.cs`
- **Purpose**: EF implementation of booking repository.
- **Quality/theory**: adapter pattern, async persistence, null-safe deletion.
- **Professor note**: includes eager loading (`Include`) for related entities; supports view/API consumers.

### `HotelBooking.Infrastructure/Repositories/CustomerRepository.cs`
- **Purpose**: EF implementation of customer repository.
- **Quality/theory**: complete CRUD adapter now replaces earlier `NotImplementedException` risk.
- **Professor note**: null-safe remove improves robustness and idempotency semantics.

### `HotelBooking.Infrastructure/Repositories/RoomRepository.cs`
- **Purpose**: EF implementation of room repository.
- **Quality/theory**: persistence boundary behavior harmonized with API expectations.
- **Professor note**: switched to null-safe deletion; avoids exception-driven flow for missing items.

---

## `HotelBooking.WebApi`

### `HotelBooking.WebApi/HotelBooking.WebApi.csproj`
- **Purpose**: API host project setup.
- **Quality/theory**: delivery layer composition; references core + infrastructure.
- **Professor note**: includes Swagger for contract discoverability.

### `HotelBooking.WebApi/Program.cs`
- **Purpose**: composition root and middleware pipeline.
- **Quality/theory**:
  - DIP wiring through DI container,
  - infrastructure chosen at edge (InMemory DB here),
  - reproducible startup seeding.
- **Professor note**: textbook example of "policy in core, mechanism in host".

### `HotelBooking.WebApi/Controllers/BookingsController.cs`
- **Purpose**: booking HTTP contract.
- **Quality/theory**:
  - maps domain outcomes to protocol semantics,
  - `400` for invalid input, `409` for state conflict, `201` for successful creation.
- **Professor note**: excellent place to explain black-box API testing and status-code correctness.

### `HotelBooking.WebApi/Controllers/RoomsController.cs`
- **Purpose**: room CRUD-ish API surface.
- **Quality/theory**: endpoint-level validation and result mapping.
- **Professor note**: `Delete` uses explicit id guard (`id > 0`); test suite validates this behavior.

### `HotelBooking.WebApi/Controllers/CustomersController.cs`
- **Purpose**: customer read endpoint.
- **Quality/theory**: minimal controller, clear single responsibility.
- **Professor note**: intentionally lightweight; can expand with details/create/update as needed.

### `HotelBooking.WebApi/appsettings.json`
### `HotelBooking.WebApi/appsettings.Development.json`
### `HotelBooking.WebApi/Properties/launchSettings.json`
- **Purpose**: environment and host behavior configuration.
- **Quality/theory**: config-as-data, environment portability.
- **Professor note**: keep secrets out of these files; use user-secrets or environment variables.

---

## `HotelBooking.Mvc`

### `HotelBooking.Mvc/HotelBooking.Mvc.csproj`
- **Purpose**: MVC UI host configuration.
- **Quality/theory**: presentation-layer adapter.
- **Professor note**: noteworthy line: `Compile Remove="Controllers\HomeController.cs"` can cause confusion if controller exists but not compiled.

### `HotelBooking.Mvc/Program.cs`
- **Purpose**: MVC composition root using SQLite.
- **Quality/theory**: same DIP wiring pattern as WebApi, different delivery mechanism.
- **Professor note**: good teaching contrast: same core logic reused across API and MVC.

### `HotelBooking.Mvc/Controllers/BookingsController.cs`
- **Purpose**: UI flow for booking create/edit/delete and calendar view.
- **Quality/theory**: orchestration controller; delegates policy to `IBookingManager`.
- **Professor note**: demonstrates separation between validation/business decision and UI feedback.

### `HotelBooking.Mvc/Controllers/RoomsController.cs`
- **Purpose**: MVC room CRUD flow.
- **Quality/theory**: repository-driven UI pattern.
- **Professor note**: has defensive id handling in delete path.

### `HotelBooking.Mvc/Controllers/CustomersController.cs`
- **Purpose**: MVC customer CRUD flow.
- **Quality/theory**: standard scaffolded pattern with concurrency handling.
- **Professor note**: strong beginner example of `ModelState` and optimistic-concurrency catch pattern.

### `HotelBooking.Mvc/Controllers/HomeController.cs`
- **Purpose**: default pages/error rendering.
- **Quality/theory**: non-functional concern (error handling, response caching metadata).
- **Professor note**: minimal boilerplate but useful for UI host completeness.

### `HotelBooking.Mvc/Models/IBookingViewModel.cs`
- **Purpose**: contract for booking calendar-style view model behavior.
- **Quality/theory**: abstraction supports controller/view decoupling.
- **Professor note**: exposes query-like properties that depend on underlying repositories.

### `HotelBooking.Mvc/Models/BookingViewModel.cs`
- **Purpose**: computes occupied dates and calendar display values.
- **Quality/theory**: view-model pattern, query aggregation.
- **Professor note**: uses `.Result` on async calls; acceptable in small demos but can risk deadlocks/perf issues in larger apps.

### `HotelBooking.Mvc/Models/ErrorViewModel.cs`
- **Purpose**: request-id display model for error UI.
- **Quality/theory**: observability and diagnosability support.
- **Professor note**: tiny but useful debugging affordance.

### `HotelBooking.Mvc/appsettings.json`
### `HotelBooking.Mvc/appsettings.Development.json`
### `HotelBooking.Mvc/Properties/launchSettings.json`
- **Purpose**: MVC host configuration.
- **Quality/theory**: configurable runtime behavior.
- **Professor note**: keep logs/environments explicit for reproducible bug reports.

---

## `HotelBooking.UnitTests`

### `HotelBooking.UnitTests/HotelBooking.UnitTests.csproj`
- **Purpose**: unit-test harness with xUnit v3 + Moq.
- **Quality/theory**: fast feedback layer in test pyramid.
- **Professor note**: confirms modern unit-testing toolchain and isolation strategy.

### `HotelBooking.UnitTests/BookingManagerTests.cs`
- **Purpose**: focused tests for core booking logic.
- **Quality/theory**:
  - AAA pattern,
  - data-driven testing with `[Theory]` + `[InlineData]`,
  - Moq setup/verify interaction testing.
- **Professor note**: strongest evidence for Part 1 requirements.

### `HotelBooking.UnitTests/RoomsControllerTests.cs`
- **Purpose**: API controller behavior tests via mock repository.
- **Quality/theory**: endpoint contract testing at unit level.
- **Professor note**: demonstrates both state assertions and interaction assertions.

### `HotelBooking.UnitTests/Fakes/FakeBookingRepository.cs`
### `HotelBooking.UnitTests/Fakes/FakeRoomRepository.cs`
- **Purpose**: legacy/manual fake repositories.
- **Quality/theory**: manual test doubles and spy-style flags (`addWasCalled`).
- **Professor note**: good for teaching double taxonomy; modern tests now mostly use Moq.

---

## `HotelBooking.IntegrationTests`

### `HotelBooking.IntegrationTests/HotelBooking.IntegrationTests.csproj`
- **Purpose**: integration test host and dependencies.
- **Quality/theory**: middle layer in test pyramid (real DB interactions).
- **Professor note**: validates seams mocks cannot catch.

### `HotelBooking.IntegrationTests/BookingManagerTests.cs`
- **Purpose**: executes `BookingManager` with real repositories + SQLite in-memory DB.
- **Quality/theory**: relational-behavior confidence (constraints/queries closer to production reality).
- **Professor note**: excellent demonstration of why integration tests complement unit tests.

---

## `HotelBooking.SpecFlowTests`

### `HotelBooking.SpecFlowTests/HotelBooking.SpecFlowTests.csproj`
- **Purpose**: BDD test host using SpecFlow + NUnit.
- **Quality/theory**: executable specifications for black-box behavior.
- **Professor note**: separates business-readable scenarios from xUnit unit tests.

### `HotelBooking.SpecFlowTests/specflow.json`
- **Purpose**: SpecFlow language/culture behavior.
- **Quality/theory**: deterministic parsing/binding behavior.
- **Professor note**: stable localization prevents scenario parser ambiguity.

### `HotelBooking.SpecFlowTests/Features/CreateBooking.feature`
- **Purpose**: Gherkin scenarios for EP/BVA on Create Booking.
- **Quality/theory**: black-box design techniques operationalized as executable specs.
- **Professor note**: scenario outlines compactly express equivalence classes and boundaries.

### `HotelBooking.SpecFlowTests/Steps/CreateBookingSteps.cs`
- **Purpose**: binding layer from Gherkin to executable C# assertions.
- **Quality/theory**: behavior-first testing, manual fake usage, scenario state management.
- **Professor note**: very good teaching example of "domain behavior over implementation detail".

### `HotelBooking.SpecFlowTests/Features/CreateBooking.feature.cs` (auto-generated)
- **Purpose**: generated glue code by SpecFlow.
- **Quality/theory**: code generation pipeline.
- **Professor note**: do not manually edit; treat as build artifact even if committed.

---

## API Automation

### `Postman/HotelBooking.postman_collection.json`
- **Purpose**: API black-box automated checks for Create Booking outcomes.
- **Quality/theory**: contract-level testing (`201`, `409`, `400`) and response-oracle validation.
- **Professor note**: strong companion to SpecFlow; validates HTTP semantics directly.

---

## Final scientific synthesis

This codebase demonstrates a strong quality argument because it combines:

1. **Design evidence**: clean architecture boundaries and DIP.
2. **Execution evidence**: unit + integration + BDD + API tests.
3. **Analytical evidence**: DD-path and cyclomatic complexity for decision-heavy methods.

That triangulation is exactly the kind of justification examiners look for in software quality courses.

