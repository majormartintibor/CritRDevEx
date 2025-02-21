using CritRDevEx.API.Contracts.Sanction;
using Wolverine;
using static CritRDevEx.API.LoanAccount.BlockAccount.BlockAccountHandler;

namespace CritRDevEx.API.Sanction.Translator;

public static class PersonSanctionedHandler
{
    public static async Task Handle(
        PersonSanctioned request,
        IMessageBus bus)
    {
        BlockAccount command = new(request.PersonId);

        await bus.SendAsync(command);
    }
}
