using Ihjezly.Application.Abstractions.Payment;
using System.Net.Http.Json;
using Ihjezly.Application.Payments.Tdsp;
using Microsoft.Extensions.Options;

namespace Ihjezly.Infrastructure.Payments.Tdsp;

public sealed class TdspPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly TdspOptions _options;

    public TdspPaymentService(HttpClient httpClient, IOptions<TdspOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.BearerToken);
    }

    public async Task<(bool IsSuccess, string? TransactionId, string? Error)> StartPaymentAsync(
        string customerId,
        decimal amount,
        string currency,
        string description)
    {
        var payload = new Dictionary<string, string>
        {
            { "id", _options.StoreId },
            { "amount", amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) },
            { "phone", customerId }, // Assuming customerId = phone
            { "email", $"{customerId}@example.com" }, // Replace with actual customer email if available
            { "backend_url", _options.BackendUrl },
            { "frontend_url", _options.FrontendUrl },
            { "custom_ref", Guid.NewGuid().ToString() }
        };

        var response = await _httpClient.PostAsync("payment/initiate", new FormUrlEncodedContent(payload));

        if (!response.IsSuccessStatusCode)
        {
            return (false, null, await response.Content.ReadAsStringAsync());
        }

        var result = await response.Content.ReadFromJsonAsync<TdspInitiateResponse>();
        if (result?.result?.Equals("success", StringComparison.OrdinalIgnoreCase) == true)
        {
            return (true, result.url, null);
        }

        return (false, null, "Failed to initiate payment");
    }

    public async Task<(bool IsSuccess, string? Result)> ConfirmPaymentAsync(string transactionId)
    {
        var payload = new Dictionary<string, string>
        {
            { "store_id", _options.StoreId },
            { "custom_ref", transactionId }
        };

        var response = await _httpClient.PostAsync("receipt/transaction", new FormUrlEncodedContent(payload));

        if (!response.IsSuccessStatusCode)
            return (false, await response.Content.ReadAsStringAsync());

        var result = await response.Content.ReadFromJsonAsync<TdspReceiptResponse>();

        if (result?.result == "success" &&
            result.data?.notes_to_shop?.payment_status?.Contains("تم استكمال") == true)
        {
            return (true, "OK");
        }

        return (false, "Not completed");
    }
}