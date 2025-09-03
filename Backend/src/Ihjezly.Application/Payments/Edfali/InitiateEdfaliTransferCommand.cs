using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Abstractions.Payment.Edfali;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Payments.Edfali;

public sealed record InitiateEdfaliTransferCommand(
    string CustomerMobile,
    decimal Amount
) : ICommand<string>;

internal sealed class InitiateEdfaliTransferHandler
    : ICommandHandler<InitiateEdfaliTransferCommand, string>
{
    private readonly IEdfaliPaymentService _edfali;

    public InitiateEdfaliTransferHandler(IEdfaliPaymentService edfali)
    {
        _edfali = edfali;
    }

    public async Task<Result<string>> Handle(InitiateEdfaliTransferCommand request, CancellationToken cancellationToken)
    {
        return await _edfali.InitiateTransferAsync(request.CustomerMobile, request.Amount);
    }
}