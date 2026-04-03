# Chapter 4 - Unit Testing with xUnit v3, Moq, and Data-Driven Design

## Learning goals

- Write quality unit tests for `BookingManager`
- Use `[Theory]` + `[InlineData]` for boundary-focused coverage
- Use Moq setup and verification correctly

## 1) xUnit v3 essentials

- `[Fact]`: one fixed scenario
- `[Theory]`: same behavior validated across multiple inputs
- `Assert.ThrowsAsync<TException>`: async exception assertions

## 2) Data-driven testing (`[Theory]` + `[InlineData]`)

### Why it exists

Boundary-heavy logic creates repetitive tests. Theories reduce duplication and show partitions clearly.

### Example: invalid ranges for `FindAvailableRoom`

```csharp
[Theory]
[InlineData(0, 0)]
[InlineData(-1, 0)]
[InlineData(2, 1)]
public async Task FindAvailableRoom_InvalidDateRange_ThrowsArgumentException(int startOffset, int endOffset)
{
    var sut = CreateManager(new List<Booking>(), new List<Room> { new Room { Id = 1, Description = "A" } });
    var startDate = DateTime.Today.AddDays(startOffset);
    var endDate = DateTime.Today.AddDays(endOffset);

    Task<int> action() => sut.FindAvailableRoom(startDate, endDate);
    await Assert.ThrowsAsync<ArgumentException>(action);
}
```

## 3) Moq mechanics

### Setup

```csharp
roomRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room>
{
    new Room { Id = 3, Description = "Suite" }
});
```

### Verify

```csharp
bookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
```

### Why this matters

- Setup controls input environment
- Verify confirms side effects and contract behavior

## 4) Test design quality checklist

- One behavior per test name
- Strong assertion (not just "not null")
- Boundary values included
- Positive and negative paths covered
- Exceptions tested on guard clauses
- Interaction assertions only when behaviorally meaningful

## 5) Recommended next extensions

- Add explicit tests for overlap edge conditions (`endDate == existing.StartDate`)
- Add tests for no rooms available (`rooms` empty)
- Add tests for inactive bookings ignored in availability calculation

## 6) Common mistakes box

- Mistake: asserting only "not null" for decision-heavy methods.
- Mistake: not verifying side effects (`AddAsync`) in create-booking tests.
- Mistake: writing many `[Fact]` tests where one `[Theory]` matrix is clearer.
- Mistake: mocking everything, including logic under test.

Fix pattern:

1. Keep `BookingManager` real.
2. Mock only repositories.
3. Assert both return value and interaction effect when relevant.

## 7) Viva mini Q&A

**Q: Why use `[Theory]` here instead of multiple `[Fact]` tests?**

**A:** Because date logic is boundary-dense. Theory matrices make partition coverage explicit and reduce duplication.

**Q: What does Moq verify add beyond pure value assertions?**

**A:** It proves required side effects (e.g., persistence call happened once, or never) and prevents false positives.

