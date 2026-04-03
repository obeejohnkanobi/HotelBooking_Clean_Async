# Chapter 7 - Quality Science Notes (for Oral Defense)

## 1) Why software quality uses layered evidence

Quality claims are stronger when supported by multiple evidence types:

- static design evidence (architecture boundaries, low coupling)
- dynamic evidence (unit/integration/API test results)
- analytical evidence (complexity and DD-path analysis)

This project intentionally uses all three.

## 2) Risk-based testing logic

Risk can be simplified as:

`Risk Exposure = Failure Probability * Failure Impact`

Applied here:

- Date-range logic has high probability of boundary mistakes -> many unit boundary tests
- Booking conflicts have high business impact -> explicit conflict API tests (`409`)
- Complex branching methods -> white-box basis path analysis

## 3) Defect classes this project targets

- **Input-validation defects**: invalid dates accepted or rejected incorrectly
- **State-transition defects**: booking saved when no room exists
- **Contract defects**: wrong HTTP status code for domain outcomes
- **Control-flow defects**: branches not executed by tests

## 4) Why mocks are not enough

Mocks are excellent for unit isolation, but they can hide integration failures:

- EF mapping issues
- DI misconfiguration
- serialization/protocol mismatches

That is why the project includes integration tests and API-level checks.

## 5) Metrics caveat (important in oral exam)

- High coverage does not guarantee correctness.
- Low complexity does not guarantee good design.
- Metrics are indicators, not proofs.

The strongest argument is: **metrics + targeted tests + clear architecture rationale**.

## 6) Suggested oral-defense framing

1. Start with architecture boundaries (why testability exists).
2. Show unit tests for decision-heavy rules and boundaries.
3. Show black-box scenarios for external behavior.
4. Show white-box complexity/path analysis for internal decision assurance.
5. Acknowledge limits and planned improvements.

