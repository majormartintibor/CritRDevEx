using CritRDevEx.API.Clock;
using CritRDevEx.API.LoanAccount.LoanAccountEvents;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.Write.CreateAccount;

public static class Endpoint
{
    public sealed record CreateLoanAccountCommand(Guid DebtorId)
    {
        public sealed class CreateLoanAccountCommandValidator : AbstractValidator<CreateLoanAccountCommand>
        {
            public CreateLoanAccountCommandValidator()
            {
                RuleFor(x => x.DebtorId).NotEmpty();
            }
        }
    }

    [WolverineBefore]
    public static async Task<ProblemDetails> CheckIfDebtorAlreadyHasAnAccount(
        CreateLoanAccountCommand command,
        IDocumentSession session)
    {
        var hasExistingAccount = await session.Events.QueryRawEventDataOnly<LoanAccountCreated>()
            .AnyAsync(e => e.DebtorId == command.DebtorId);

        if (hasExistingAccount)
            return new ProblemDetails
            {
                Detail = "Debtor already has an account",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    public const string CreateLoanAccountEndpoint = "/api/loanAccount/create";
    private const decimal defaultLimit = -30000;

    [Tags(Tag.LoanAccount)]
    [WolverinePost(CreateLoanAccountEndpoint)]
    public static (IResult, IStartStream) CreateNewAccount(
        CreateLoanAccountCommand command)
    {
        LoanAccountCreated created = new(command.DebtorId, defaultLimit, DateTimeProvider.UtcNow);

        var open = MartenOps.StartStream<LoanAccount>(created);

        return (Results.Ok(open.StreamId), open);
    }
}
