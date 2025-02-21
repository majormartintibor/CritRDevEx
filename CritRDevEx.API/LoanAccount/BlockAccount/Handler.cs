using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Wolverine;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.BlockAccount;

public static class BlockAccountHandler
{
    public sealed record BlockAccount(Guid AccountId)
    {
        public sealed class BlockLoanAccountValidator : AbstractValidator<BlockAccount>
        {
            public BlockLoanAccountValidator()
            {
                RuleFor(x => x.AccountId).NotEmpty();
            }
        }
    }
   
    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
        BlockAccount request,
        [Required] Account account)
    {
        Events events = [];
        OutgoingMessages messages = [];

        if (account.AccountStatus == AccountStatus.Blocked)
            throw new InvalidOperationException("Account is already blocked");

        events.Add(new AccountBlocked(request.AccountId));

        return (events, messages);
    }
}
