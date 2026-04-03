# Common Student Mistakes (and How to Fix Them)

## Part 1 - Unit Testing

### Mistake 1: Weak assertions
- **Symptom**: tests only check `NotNull` or `NotEqual(-1)`.
- **Risk**: false confidence.
- **Fix**: assert exact business outcomes and interaction effects (`Times.Once`, `Times.Never()`).

### Mistake 2: Mixing Arrange/Act/Assert
- **Symptom**: setup and assertion logic interleaved.
- **Risk**: poor diagnosability.
- **Fix**: strict AAA sections in each test.

### Mistake 3: Over-mocking internals
- **Symptom**: verifying every call and argument detail.
- **Risk**: brittle tests that fail on harmless refactors.
- **Fix**: verify only behaviorally meaningful interactions.

### Mistake 4: Missing boundary cases
- **Symptom**: only mid-range valid dates tested.
- **Risk**: edge bugs survive.
- **Fix**: include boundaries (`today`, `start > end`, adjacent intervals).

## Part 2 - Black-box Testing

### Mistake 5: Confusing EP with random testing
- **Symptom**: arbitrary cases, no partition logic.
- **Risk**: poor requirement traceability.
- **Fix**: explicitly define partitions (valid+available, valid+unavailable, invalid).

### Mistake 6: BVA without true edges
- **Symptom**: boundaries mentioned but not executed.
- **Risk**: important defect zones untested.
- **Fix**: include exact threshold values and just-inside/just-outside cases.

### Mistake 7: SpecFlow steps test internals
- **Symptom**: step definitions inspect private implementation details.
- **Risk**: violates black-box intent.
- **Fix**: assert observable outcomes only.

## Part 3 - White-box Testing

### Mistake 8: Miscounting cyclomatic complexity
- **Symptom**: forgetting loop predicates or compound decisions.
- **Risk**: incorrect oral defense.
- **Fix**: count all decision points consistently; cross-check with graph formula.

### Mistake 9: Treating all paths as feasible
- **Symptom**: listing impossible execution paths.
- **Risk**: weak technical credibility.
- **Fix**: validate path feasibility against guard clauses and control flow.

### Mistake 10: Coverage metric absolutism
- **Symptom**: "high coverage means no bugs."
- **Risk**: overclaiming quality.
- **Fix**: explain coverage as indicator, not proof; combine with architecture and risk analysis.

