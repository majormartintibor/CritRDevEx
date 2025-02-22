﻿using FluentValidation;
using Marten;
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

    public static async Task<bool> LoadAsync(CreateLoanAccount request, IQuerySession session)
    {
        return await session.Query<Account>()
            .AnyAsync(account => account.DebtorId == request.DebtorId);
    }

    public const string CreateAccountEnpoint = "/api/loanAccount/create";
    private const decimal defaultLimit = -30000;

    [Tags(Tag.LoanAccount)]
    [WolverinePost(CreateAccountEnpoint)]
    public static (IResult, IStartStream) CreateNewAccount(
        CreateLoanAccount request,
        bool hasExistingAccount)
    {
        if (hasExistingAccount)
            throw new InvalidOperationException("Debtor already has an account");

        LoanAccountCreated created = new(request.DebtorId, defaultLimit);

        var open = MartenOps.StartStream<Account>(created);

        return (Results.Ok(open.StreamId),open);        
    }
}
