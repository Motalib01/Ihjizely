using Ihjezly.Application.Abstractions.Payment;
using Ihjezly.Application.Payments.Masarat;
using Ihjezly.Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Ihjezly.Infrastructure.Payments.Masarat;


public class MasaratPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MasaratPaymentService> _logger;
    private readonly MasaratOptions _options;

    public MasaratPaymentService(HttpClient httpClient, ILogger<MasaratPaymentService> logger, IOptions<MasaratOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<(bool IsSuccess, string? TransactionId, string? Error)> StartPaymentAsync(
        string customerId, decimal amount, string currency, string description)
    {
        try
        {
            // Step 1: Sign in
            var signInResponse = await _httpClient.PostAsJsonAsync("/api/OnlinePaymentServices/Signin", new
            {
                userId = _options.UserId,
                pin = _options.Pin,
                providerId = _options.ProviderId,
                authUserType = _options.AuthUserType
            });

            if (!signInResponse.IsSuccessStatusCode)
            {
                var message = await signInResponse.Content.ReadAsStringAsync();
                _logger.LogWarning("Masarat SignIn failed: {Message}", message);
                return (false, null, "SignIn failed");
            }

            var signInContent = await signInResponse.Content.ReadFromJsonAsync<MasaratSignInResponse>();
            if (signInContent?.Type != 1)
                return (false, null, "Authentication failed: " + string.Join(" | ", signInContent?.Messages ?? []));

            // Step 2: Open session
            var transactionId = new Random().Next(100000, 999999);

            var openSessionResponse = await _httpClient.PostAsJsonAsync("/api/OnlinePaymentServices/OpenSession", new
            {
                identityCard = customerId,
                amount,
                transactionId,
                onlineOperation = 1 // Pay
            });

            if (!openSessionResponse.IsSuccessStatusCode)
            {
                var msg = await openSessionResponse.Content.ReadAsStringAsync();
                _logger.LogWarning("Masarat OpenSession failed: {Message}", msg);
                return (false, null, "OpenSession failed");
            }

            var sessionResponse = await openSessionResponse.Content.ReadFromJsonAsync<MasaratGenericResponse>();
            if (sessionResponse?.Type != 1)
                return (false, null, "OpenSession error: " + string.Join(" | ", sessionResponse?.Messages ?? []));

            return (true, transactionId.ToString(), null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Masarat payment start");
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool IsSuccess, string? Result)> ConfirmPaymentAsync(string transactionId)
    {
        try
        {
            
            var otp = MasaratOtpProvider.GetOtp();

            var response = await _httpClient.PostAsJsonAsync("/api/OnlinePaymentServices/CompleteSession?culture=ar-LY", new
            {
                otp
            });

            if (!response.IsSuccessStatusCode)
                return (false, "Confirm failed: " + await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadFromJsonAsync<MasaratGenericResponse>();
            return result?.Type == 1
                ? (true, result?.Content ?? "Success")
                : (false, string.Join(" | ", result?.Messages ?? new List<string> { "Unknown error" }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Masarat payment confirmation");
            return (false, ex.Message);
        }
    }
}


// === Response DTOs ===