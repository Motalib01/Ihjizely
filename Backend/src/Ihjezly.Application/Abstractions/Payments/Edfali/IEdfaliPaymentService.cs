namespace Ihjezly.Application.Abstractions.Payment.Edfali;

public interface IEdfaliPaymentService
{
    /// <summary>
    /// Initiate transfer request (DoPTrans)
    /// </summary>
    /// <param name="customerMobile">Customer phone (+2189xxxx...)</param>
    /// <param name="amount">Amount to transfer</param>
    /// <returns>sessionId if success, otherwise error code (PW1, PW, Limit, ACC, Bal)</returns>
    Task<string> InitiateTransferAsync(string customerMobile, decimal amount);

    /// <summary>
    /// Confirm transfer request (OnlineConfTrans)
    /// </summary>
    /// <param name="confirmationPin">PIN received via SMS by the customer</param>
    /// <param name="sessionId">sessionId returned from DoPTrans</param>
    /// <returns>"OK" if success, otherwise error code</returns>
    Task<string> ConfirmTransferAsync(string confirmationPin, string sessionId);
}