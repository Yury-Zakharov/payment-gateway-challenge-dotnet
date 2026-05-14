## Main assumptions
- No extra libraries, to keep common background
- I kept dotnet version, 8 as I understand that not every system always use latest or even LTS.
- Auth, rate limiting, logging and persistence are out of scope here.
- I used random set of currencies
## Core design choices
- No shared types between boundaries.
- Validation through data annotations and `IValidatableObject`. Model validator shared between code and tests, Endpoint filter is used due to .NET8 limitations
- Testing: as much as possible shifted to unit tests, test containers in integration tests for better isolation; Created only critical required tests.
- Changes in contracts (comparing to the template), to support common sense:
    - Amount: `long` instead of `int` to avoid overflow, formally still comply with the spec as integral number
    - Card number: `string` instead of `int`, avoiding overflow issue
    - Last four digits of card number: `string` instead of `int` to avoid leading/trailing zeroes issue
    - Currency: `enum` instead of `string` for better validation

Minor decisions and choices are explained in comments in the code, I tried to make my code clear without comments.

