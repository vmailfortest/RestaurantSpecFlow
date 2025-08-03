using System.Globalization;
using ApiTests.api;
using ApiTests.helper;
using ApiTests.model;
using FluentAssertions;
using TechTalk.SpecFlow;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ApiTests.steps;

[Binding]
public class OrderSteps
{
    private readonly ScenarioContext _scenario;
    private readonly ITestOutputHelper _output;
    private readonly WireMockOrderStub _mock;
    private readonly OrderApiClient _orderClient;

    public OrderSteps(ScenarioContext scenario, ITestOutputHelper output)
    {
        _scenario = scenario;
        _output = output;
        _mock = new WireMockOrderStub();
        _orderClient = new OrderApiClient(_mock.Url);
    }

    [Given(@"wiremock testId is (.*)")]
    public void GivenWiremockTestIdIs(string testId)
    {
        _scenario["testId"] = testId;
    }

    [Given(@"new order with the following items is created")]
    public async Task GivenNewOrderWithTheFollowingItemsIsCreated(Table table)
    {
        string testId = _scenario["testId"].ToString();

        Order order = new Order { Dishes = ParseDishes(table) };

        var response = await _orderClient.CreateOrder(order, testId);

        order.Id = response.OrderId;
        _scenario["currentOrder"] = order;

        _output.WriteLine($"---orderId: {response.OrderId.ToString()}");
    }

    [When(@"add to order")]
    public async Task WhenAddToOrder(Table table)
    {
        string testId = _scenario["testId"].ToString();
        Order currentOrder = (Order)_scenario["currentOrder"];

        foreach (var dish in ParseDishes(table))
        {
            var existing = currentOrder.Dishes.FirstOrDefault(d =>
                d.DishType == dish.DishType &&
                d.OrderedAt == dish.OrderedAt);

            if (existing != null)
            {
                existing.Quantity += dish.Quantity;
            }
            else
            {
                currentOrder.Dishes.Add(dish);
            }
        }

        await _orderClient.UpdateOrder(currentOrder, testId);

        _scenario["currentOrder"] = currentOrder;
    }

    [When(@"remove from order")]
    public async Task WhenRemoveFromOrder(Table table)
    {
        string testId = _scenario["testId"].ToString();
        Order currentOrder = (Order)_scenario["currentOrder"];

        foreach (var row in table.Rows)
        {
            var dishType = Enum.Parse<DishType>(row["Type"]);
            var time = DateTime.Parse(row["Time"]);

            currentOrder.Dishes.RemoveAll(d =>
                d.DishType == dishType &&
                d.OrderedAt == time);
        }

        await _orderClient.UpdateOrder(currentOrder, testId);

        _scenario["currentOrder"] = currentOrder;
    }

    [When(@"get bill")]
    public async Task WhenGetBill()
    {
        string testId = _scenario["testId"].ToString();
        Order currentOrder = (Order)_scenario["currentOrder"];

        var response = await _orderClient.CalcOrder(currentOrder.Id, testId);

        _scenario["orderCalc"] = response.Total;
    }

    [Then(@"bill should be (.*)")]
    public void ThenBillShouldBe(string expected)
    {
        var actual = _scenario["orderCalc"].ToString();

        actual.Should().Be(expected);
    }

    private List<Dish> ParseDishes(Table table)
    {
        return table.Rows.Select(row => new Dish
        {
            DishType = Enum.Parse<DishType>(row["Type"]),
            Quantity = int.Parse(row["Quantity"]),
            OrderedAt = DateTime.Parse(row["Time"])
        }).ToList();
    }
}