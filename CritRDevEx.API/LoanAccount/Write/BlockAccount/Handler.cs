using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.BlockAccount;

public static class BlockLoanAccountHandler
{
    public sealed record BlockLoanAccount(Guid LoanAccountId)
    {
        public sealed class BlockLoanAccountValidator : AbstractValidator<BlockLoanAccount>
        {
            public BlockLoanAccountValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
            }
        }
    }

    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
        BlockLoanAccount request,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == LoanAccountStatus.Blocked)
            throw new InvalidOperationException("Account is already blocked");

        events.Add(new LoanAccountBlocked(request.LoanAccountId, DateTimeProvider.UtcNow));

        return (events, messages);
    }
}
