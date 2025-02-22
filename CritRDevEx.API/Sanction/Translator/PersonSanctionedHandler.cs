using CritRDevEx.API.Contracts.Sanction;
using Wolverine;
using static CritRDevEx.API.LoanAccount.BlockAccount.BlockLoanAccountHandler;

namespace CritRDevEx.API.Sanction.Translator;

/*
 * If there was any business logic than it would not be in here but in a pure function 
 * This is the Handler in the A-Frame as it is responsible for I/O
 * Hence there is no unit test for this class, only integration tests
 */
public static class PersonSanctionedHandler
{
    public static async Task Handle(
        PersonSanctioned request,
        IMessageBus bus)
    {
        BlockLoanAccount command = new(request.PersonId);

        await bus.SendAsync(command);
    }
}
