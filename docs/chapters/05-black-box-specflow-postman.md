# Chapter 5 - Black-box Testing (EP/BVA), SpecFlow, and Postman

## Learning goals

- Derive black-box test cases for Create Booking
- Understand Equivalence Partitioning (EP) and Boundary Value Analysis (BVA)
- Implement BDD in SpecFlow and endpoint checks in Postman

## 1) Equivalence Partitioning (EP)

### Definition

Divide input domain into classes expected to behave the same.

### Create Booking partitions

- Valid date range + room available -> booking created
- Valid date range + no room available -> conflict/failure
- Invalid date range -> bad request/exception path

## 2) Boundary Value Analysis (BVA)

### Definition

Test around edges where bugs cluster.

### Key boundaries in this project

- `startDate == DateTime.Today` (invalid)
- `startDate == endDate` (single-day booking)
- `startDate > endDate` (invalid)
- Adjacent booking windows (`endDate < existing.StartDate` and `startDate > existing.EndDate`)

## 3) Gherkin + SpecFlow in this repo

Feature file:

```gherkin
Scenario Outline: Create Booking with EP/BVA partitions
  Given the hotel has 2 rooms
  And active bookings from <existingStartOffset> to <existingEndOffset> in both rooms
  When I request a booking from <requestStartOffset> to <requestEndOffset>
  Then booking creation should be <expectedResult>
```

Step definitions bind plain language to C# methods in `HotelBooking.SpecFlowTests/Steps/CreateBookingSteps.cs`.

## 4) How Cucumber-style flow works in C#

- Gherkin sentence is parsed
- Matching `[Given]`, `[When]`, `[Then]` method is invoked
- Shared scenario state is stored in step class fields
- Assertions verify externally visible behavior

## 5) Postman API automation

The collection `Postman/HotelBooking.postman_collection.json` includes:

- `Create Booking - Success (201)`
- `Create Booking - Conflict (409)`
- `Create Booking - Invalid Date (400)`

Example assertion script:

```javascript
pm.test('Status code is 409', function () {
  pm.response.to.have.status(409);
});
pm.test('Conflict message explains no availability', function () {
  pm.expect(pm.response.text()).to.include('could not be created');
});
```

## 6) Trade-offs

- SpecFlow improves stakeholder readability but adds tooling overhead
- Postman validates contract behavior, but is weaker than unit tests for algorithm-level root-cause diagnosis

