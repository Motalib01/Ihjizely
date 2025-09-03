using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Abstractions.Payment.Edfali;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Payments.Edfali;

public sealed record ConfirmEdfaliTransferCommand(
    string ConfirmationPin,
    string SessionId
) : ICommand<string>;


internal sealed class ConfirmEdfaliTransferHandler
    : ICommandHandler<ConfirmEdfaliTransferCommand, string>
{
    private readonly IEdfaliPaymentService _edfali;

    public ConfirmEdfaliTransferHandler(IEdfaliPaymentService edfali)
    {
        _edfali = edfali;
    }

    public async Task<Result<string>> Handle(ConfirmEdfaliTransferCommand request, CancellationToken cancellationToken)
    {
        return await _edfali.ConfirmTransferAsync(request.ConfirmationPin, request.SessionId);
    }
}