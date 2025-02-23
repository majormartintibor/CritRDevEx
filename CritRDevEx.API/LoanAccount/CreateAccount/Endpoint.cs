using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace CritRDevEx.API.LoanAccount.CreateAccount;

public static class Endpoint
{
    public sealed record CreateLoanAccount(Guid DebtorId)
    {
        public sealed class CreateLoanAccountValidator : AbstractValidator<CreateLoanAccount>
        {
            public CreateLoanAccountValidator()
            {
                RuleFor(x => x.DebtorId).NotEmpty();
            }
        }
    }

    [WolverineBefore]
    public static async Task<ProblemDetails> CheckIfDebtorAlreadyHasAnAccount(
        CreateLoanAccount request,
        IQuerySession querySession)
    {
        var hasExistingAccount = await querySession.Query<LoanAccount>()
            .AnyAsync(account => account.DebtorId == request.DebtorId);

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
        CreateLoanAccount request)
    { 
        LoanAccountCreated created = new(request.DebtorId, defaultLimit);

        var open = MartenOps.StartStream<LoanAccount>(created);

        return (Results.Ok(open.StreamId),open);        
    }    
}
