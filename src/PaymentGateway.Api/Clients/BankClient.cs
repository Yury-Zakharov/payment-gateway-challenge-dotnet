using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PaymentGateway.Api.Models.Configuration;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Clients;

public sealed class BankClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public BankClient(HttpClient httpClient, IOptions<BankClientOptions> options, ILogger<BankClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<BankPaymentResponse> PostPaymentAsync(BankPaymentRequest request, CancellationToken ct = default)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("/payments", request, JsonOptions, ct);

        if (httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            _logger.LogError("Bank call failed for card ending {LastFour}",
                request.CardNumber[^4..]);
            
            throw new HttpRequestException(
                "Bank is unavailable", 
                null, 
                HttpStatusCode.ServiceUnavailable);
        }

        _logger.LogInformation("Bank call completed for card ending {LastFour}",
             request.CardNumber[^4..]);
        
        httpResponse.EnsureSuccessStatusCode();

        return await httpResponse.Content.ReadFromJsonAsync<BankPaymentResponse>(JsonOptions, ct)
               ?? throw new InvalidOperationException("Invalid bank response");
    }
}