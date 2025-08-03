using WireMock.ResponseBuilders;
using WireMock.Server;
using static WireMock.RequestBuilders.Request;

namespace ApiTests.helper;

public class WireMockOrderStub : IDisposable
{
    private readonly WireMockServer _server;
    public string Url => _server.Urls[0];

    public WireMockOrderStub()
    {
        _server = WireMockServer.Start();

        SetupCreateOrder();
        SetupUpdateOrder();
        SetupGetBill();
    }

    private void SetupCreateOrder()
    {
        _server
            .Given(Create().WithPath("/order").UsingPost()
                .WithHeader("X-Test-Id", "test1"))
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{ \"OrderId\": 101}"));

        _server
            .Given(Create().WithPath("/order").UsingPost()
                .WithHeader("X-Test-Id", "test2"))
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{ \"OrderId\": 102}"));

        _server
            .Given(Create().WithPath("/order").UsingPost()
                .WithHeader("X-Test-Id", "test3"))
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{ \"OrderId\": 103}"));
    }

    private void SetupUpdateOrder()
    {
        _server
            .Given(Create().WithPath("/order/*").UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(200));
    }

    private void SetupGetBill()
    {
        // Scenario 1
        _server
            .Given(Create().WithPath("/order/101/calc").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"Total\": 58.40 }"));

        // Scenario 2
        _server
            .Given(Create().WithPath("/order/102/calc").UsingGet())
            .InScenario("OrderCalc").WillSetStateTo("afterUpdate")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"Total\": 23.30 }"));

        _server
            .Given(Create().WithPath("/order/102/calc").UsingGet())
            .InScenario("OrderCalc").WhenStateIs("afterUpdate")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"Total\": 43.70 }"));

        // Scenario 3
        _server
            .Given(Create().WithPath("/order/103/calc").UsingGet())
            .InScenario("OrderCalc").WillSetStateTo("afterUpdate")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"Total\": 58.40 }"));

        _server
            .Given(Create().WithPath("/order/103/calc").UsingGet())
            .InScenario("OrderCalc").WhenStateIs("afterUpdate")
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ \"Total\": 43.80 }"));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}