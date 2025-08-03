# 🍽️ Restaurant Checkout System – API Tests

This project contains automated API tests for restaurant checkout system.

---

## Tech Stack

- **C# (.NET 8)**
- **SpecFlow** for BDD
- **xUnit** as test runner
- **RestSharp** as HTTP client
- **FluentAssertions** for assertions
- **WireMock.Net** to mock REST API responses

## How to Run

- Clone the repository
- Navigate to the test project directory:
    ```bash
    cd RestaurantAutotests/ApiTests
    ```
- Run tests
    ```bash
    dotnet restore
    dotnet test
    ```

## Assumptions
- When order time is not specified, bill is calculated without discount.
- 10% service charge calculated for food only (starters and mains).

## Implementaion specifics
- PUT /order/{id} request replaces the entire order.
- Equality of dishes is determined by DishType and OrderedAt time.
- Each test scenario use X-Test-Id header, so WireMock return different mocked responses on the same endpoints, depending on the flow.

## Project Structure
```
RestaurantAutotests/
└── ApiTests/
    ├── src/
    │   ├── api/                # OrderApiClient.cs (REST client)
    │   ├── helper/             # WireMock stubbing logic
    │   ├── model/              # DTOs: Order, Dish, etc.
    │   ├── steps/              # SpecFlow step definitions
    │   └── tests/              # Gherkin feature files
```