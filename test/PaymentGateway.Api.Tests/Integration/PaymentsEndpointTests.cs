using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Integration;

public sealed class PaymentsEndpointTests(PaymentGatewayWebApplicationFactory factory)
    : IClassFixture<PaymentGatewayWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task PostPayment_AuthorizedCard_Returns200WithPayment()
    {
        var request = CreateValidRequest("4242424242424241");

        var response = await _client.PostAsJsonAsync("/payments", request, new CancellationToken(false));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));
        Assert.Equal("Authorized", payment.GetProperty("status").GetString());
        Assert.Equal("4241", payment.GetProperty("lastFourCardDigits").GetString());
    }

    [Fact]
    public async Task PostPayment_DeclinedCard_Returns200WithPayment()
    {
        var request = CreateValidRequest("4242424242424242");
        var response = await _client.PostAsJsonAsync("/payments", request, new CancellationToken(false));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payment = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));
        Assert.Equal("Declined", payment.GetProperty("status").GetString());
        Assert.Equal("4242", payment.GetProperty("lastFourCardDigits").GetString());
    }

    [Fact]
    public async Task PostPayment_CardEndsWithZero_Returns503BankError()
    {
        var request = CreateValidRequest("4242424242424240");

        var response = await _client.PostAsJsonAsync("/payments", request, new CancellationToken(false));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData("1234567890123", "Card number must be at least 14 characters long.")]
    [InlineData("12345678901234567890", "Card number must be at most 19 characters long.")]
    [InlineData("4242-4242-4242-4242", "Card number must only contain numeric characters.")]
    [InlineData("", "Card number must only contain numeric characters.")]
    public async Task PostPayment_InvalidCardNumber_Returns400(string cardNumber, string _)
    {
        var request = CreateValidRequest(cardNumber);

        var response = await _client.PostAsJsonAsync("/payments", request, new CancellationToken(false));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPayment_ExistingPayment_Returns200()
    {
        var createRequest = CreateValidRequest("4242424242424241");
        var createResponse = await _client.PostAsJsonAsync("/payments", createRequest, new CancellationToken(false));
        var paymentJson =
            await createResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));
        var id = paymentJson.GetProperty("id").GetString()!;

        var getResponse = await _client.GetAsync($"/payments/{id}", new CancellationToken(false));

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrieved =
            await getResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));
        Assert.Equal(id, retrieved.GetProperty("id").GetString());
        Assert.Equal("Authorized", retrieved.GetProperty("status").GetString());
    }

    [Fact]
    public async Task GetPayment_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/payments/{Guid.NewGuid()}", new CancellationToken(false));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPayment_Successful_Roundtrip_GetReturnsExactPayment()
    {
        var request = CreateValidRequest("4242424242424241");

        var postResponse = await _client.PostAsJsonAsync("/payments", request, new CancellationToken(false));
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var created =
            await postResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));
        var id = created.GetProperty("id").GetString()!;

        var getResponse = await _client.GetAsync($"/payments/{id}", new CancellationToken(false));
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrieved =
            await getResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions, new CancellationToken(false));

        Assert.Equal(id, retrieved.GetProperty("id").GetString());
        Assert.Equal("Authorized", retrieved.GetProperty("status").GetString());
        Assert.Equal("4241", retrieved.GetProperty("lastFourCardDigits").GetString());
        Assert.Equal(request.ExpiryMonth, retrieved.GetProperty("expiryMonth").GetInt32());
        Assert.Equal(request.ExpiryYear, retrieved.GetProperty("expiryYear").GetInt32());
        Assert.Equal(request.Currency.ToString(), retrieved.GetProperty("currency").GetString());
        Assert.Equal(request.Amount, retrieved.GetProperty("amount").GetInt64());
    }

    private static PostPaymentRequest CreateValidRequest(string cardNumber) => new()
    {
        CardNumber = cardNumber,
        ExpiryMonth = 12,
        ExpiryYear = DateTime.UtcNow.Year + 29,
        Currency = Currency.GBP,
        Amount = 1000,
        Cvv = "123"
    };
}