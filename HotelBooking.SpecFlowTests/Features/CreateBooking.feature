Feature: Create Booking
  As a booking client
  I want to create a booking only when a room is available and dates are valid
  So that the system never creates conflicting reservations

  Scenario Outline: Create Booking with EP/BVA partitions
    Given the hotel has 2 rooms
    And active bookings from <existingStartOffset> to <existingEndOffset> in both rooms
    When I request a booking from <requestStartOffset> to <requestEndOffset>
    Then booking creation should be <expectedResult>

    Examples:
      | existingStartOffset | existingEndOffset | requestStartOffset | requestEndOffset | expectedResult |
      | 5                   | 8                 | 6                  | 7                | false          |
      | 5                   | 8                 | 9                  | 10               | true           |
      | 5                   | 8                 | 5                  | 8                | false          |

  Scenario Outline: Create Booking with invalid ranges
    Given the hotel has 1 rooms
    And no existing active bookings
    When I request a booking from <requestStartOffset> to <requestEndOffset>
    Then the request should be rejected as invalid

    Examples:
      | requestStartOffset | requestEndOffset |
      | 0                  | 0                |
      | -1                 | 1                |
      | 3                  | 2                |

