using ApiTests.model;
using FluentAssertions;
using RestSharp;

namespace ApiTests.api;

public class OrderApiClient
{
    readonly RestClient _client;

    public OrderApiClient(string url)
    {
        var options = new RestClientOptions(url);
        _client = new RestClient(options);
    }

    public async Task<CreateOrderResponse> CreateOrder(Order order, string testId)
    {
        var request = new RestRequest("/order")
            .AddHeader("X-Test-Id", testId)
            .AddJsonBody(order);
        var response = await _client.PostAsync<CreateOrderResponse>(request);

        return response;
    }

    public async Task<RestResponse> UpdateOrder(Order order, string testId)
    {
        var request = new RestRequest($"/order/{order.Id.ToString()}")
            .AddHeader("X-Test-Id", testId)
            .AddJsonBody(order);
        var response = await _client.PutAsync(request);

        return response;
    }

    public async Task<CalcOrderResponse> CalcOrder(int id, string testId)
    {
        var request = new RestRequest($"/order/{id}/calc")
            .AddHeader("X-Test-Id", testId);
        CalcOrderResponse response = await _client.GetAsync<CalcOrderResponse>(request);
        return response;
    }
}