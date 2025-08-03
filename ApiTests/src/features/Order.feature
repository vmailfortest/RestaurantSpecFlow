Feature: Order creation

    Scenario: Group of 4 orders 4 starters, 4 mains, and 4 drinks after 19:00
        Given wiremock testId is test1
        Given new order with the following items is created
          | Type    | Quantity | Time  |
          | Starter | 4        | 20:00 |
          | Main    | 4        | 20:00 |
          | Drink   | 4        | 20:00 |
        When get bill
        Then bill should be 58.40

    Scenario: Add dishes to existing order
        Given wiremock testId is test2
        Given new order with the following items is created
          | Type    | Quantity | Time  |
          | Starter | 1        | 18:00 |
          | Main    | 2        | 18:00 |
          | Drink   | 2        | 18:00 |
        When get bill
        Then bill should be 23.30
        When add to order
          | Type  | Quantity | Time  |
          | Main  | 2        | 18:00 |
          | Drink | 2        | 20:00 |
        When get bill
        Then bill should be 43.70

    Scenario: Remove dishes from existing order
        Given wiremock testId is test3
        Given new order with the following items is created
          | Type    | Quantity | Time  |
          | Starter | 4        | 18:00 |
          | Main    | 4        | 18:00 |
          | Drink   | 4        | 18:00 |
        When get bill
        Then bill should be 58.40
        When remove from order
          | Type    | Quantity | Time  |
          | Starter | 1        | 18:00 |
          | Main    | 1        | 18:00 |
          | Drink   | 1        | 18:00 |
        And get bill
        Then bill should be 43.80