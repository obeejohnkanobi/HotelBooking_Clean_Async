# Viva Question Bank (with Model Answers)

## Architecture and Design

### Q1. Why must `HotelBooking.Core` avoid references to Infrastructure?
**Model answer**: To preserve dependency direction and testability. Core contains policy and should not depend on mechanisms like EF Core. This enables unit testing core logic with mocks/fakes and prevents framework coupling.

### Q2. What is the practical benefit of DIP in this project?
**Model answer**: `BookingManager` depends on `IRepository<T>`, so tests can swap real repositories with Moq or in-memory doubles. The same business logic works with different delivery mechanisms (MVC/API).

## Unit Testing

### Q3. Difference between `[Fact]` and `[Theory]` in xUnit?
**Model answer**: `[Fact]` is one fixed test case. `[Theory]` runs the same test behavior across multiple inputs (`[InlineData]`), ideal for boundary/equivalence coverage.

### Q4. When should you verify interactions with Moq?
**Model answer**: When side effects are part of required behavior, e.g., verifying `AddAsync` is called once on successful booking and never called when no room is available.

### Q5. Fake vs Mock - which did you use and why?
**Model answer**: Unit tests mainly use Moq mocks for precise interaction checks. SpecFlow steps use manual in-memory fakes for readable, stateful scenario execution.

## Black-box Testing

### Q6. Explain EP and BVA for Create Booking.
**Model answer**: EP groups inputs into behaviorally equivalent classes: valid+available, valid+occupied, invalid range. BVA targets edges: `start == today`, `start > end`, and adjacency around existing bookings.

### Q7. Why test both SpecFlow and Postman?
**Model answer**: SpecFlow captures business-readable behavior scenarios; Postman validates HTTP contract semantics and response codes/payloads from client perspective.

## White-box Testing

### Q8. How did you compute cyclomatic complexity?
**Model answer**: I used predicate count + 1 and cross-checked with `E - N + 2`. For `FindAvailableRoom`, M=4. For `GetFullyOccupiedDates`, M=5.

### Q9. What is a DD-path graph and why use it?
**Model answer**: It is a condensed control-flow graph that highlights decisions and transfer paths. It helps derive independent basis paths and justify white-box test design.

### Q10. Does high coverage prove correctness?
**Model answer**: No. Coverage indicates explored execution space, not semantic correctness. Quality confidence comes from combining strong oracles, risk-based cases, architecture rationale, and analytical checks.

