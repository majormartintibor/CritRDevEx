using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.BlockAccount;

public static class BlockLoanAccountCommandHandler
{
    public sealed record BlockLoanAccountCommand(Guid LoanAccountId)
    {
        public sealed class BlockLoanAccountCommandValidator : AbstractValidator<BlockLoanAccountCommand>
        {
            public BlockLoanAccountCommandValidator()
            {
                RuleFor(x => x.LoanAccountId).NotEmpty();
            }
        }
    }

    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
        BlockLoanAccountCommand command,
        [Required] LoanAccount account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus != LoanAccountStatus.Blocked)
            events.Add(new LoanAccountBlocked(command.LoanAccountId, DateTimeProvider.UtcNow));

        return (events, messages);
    }
}
