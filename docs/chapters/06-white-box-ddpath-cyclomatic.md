# Chapter 6 - White-box Testing, DD-path Graphs, and Cyclomatic Complexity

## Learning goals

- Distinguish statement, branch, and path coverage
- Build DD-path graphs from real code
- Compute cyclomatic complexity and defend values orally

## 1) Coverage models

- **Statement coverage**: execute each line at least once
- **Branch coverage**: execute each decision outcome (`true` and `false`)
- **Path coverage**: execute unique decision paths (highest effort)

## 2) DD-path graph construction process

1. Mark decision nodes (`if`, loop conditions, short-circuit logic)
2. Collapse linear statements into single nodes
3. Connect edges for all possible control transfers
4. Identify entry/exit nodes
5. Enumerate independent basis paths

### Science note

DD-path analysis comes from control-flow graph theory. The objective is to model all
possible execution transfers, then derive a minimal independent path set that guarantees
decision-structure coverage.

## 3) Cyclomatic complexity formulas

- Graph method: `M = E - N + 2P` (for one connected component, `P = 1`)
- Predicate method: `M = predicate_count + 1`

Both formulas should agree for the same control-flow graph.

Interpretation guideline:

- `M` approximates the number of independent decision paths.
- Higher `M` implies higher minimum testing effort and usually higher maintenance risk.

## 4) Applied to this project

From `HotelBooking.Core/Services/BookingManager.cs`:

- `FindAvailableRoom` complexity = **4**
- `GetFullyOccupiedDates` complexity = **5**

Detailed graphs and basis paths are in `docs/white-box-analysis.md`.

Why these values matter:

- `4` and `5` are moderate complexity values: manageable, but enough branching to justify
  deliberate boundary tests instead of ad hoc random cases.

## 5) Short oral-defense script

- "I first extracted decision points from guard clauses, loops, and conditionals."
- "Then I built DD-path graphs and computed complexity using predicate count + 1."
- "I cross-checked with `E - N + 2` and obtained the same values."
- "My basis paths map directly to tests that exercise throw, early-return, and loop branches."

## 6) Practical warning

High cyclomatic complexity does not automatically mean bad code. It signals where additional tests and simplification reviews are most valuable.

Path coverage warning:

- Full path coverage quickly becomes impractical with loops (potentially infinite paths).
- In practice, teams target branch coverage + basis path coverage for strong but feasible assurance.

## 7) Common pitfalls

- Forgetting to count loop predicates in cyclomatic complexity.
- Counting infeasible paths as if they are executable.
- Claiming path coverage is complete without proving path feasibility.
- Treating complexity as a quality verdict instead of a risk indicator.

## 8) Viva mini Q&A

**Q: Why did you compute complexity using two formulas?**

**A:** Cross-checking reduces counting errors. Predicate count + 1 and `E - N + 2` should match for the same graph.

**Q: If two methods have moderate complexity, why still do white-box analysis?**

**A:** Because moderate complexity still hides branch-specific defects. White-box analysis ensures decision paths are intentionally exercised, not accidentally covered.

