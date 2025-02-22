using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.History;

public static class Endpoint
{
    public const string GetAccountHistoryEnpoint = "/api/loanAccount/history/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetAccountHistoryEnpoint + "{accountId:guid}")]
    public static Task GetInspection([FromRoute] Guid accountId, IQuerySession querySession, CancellationToken ct)
        => querySession.Query<AccountHistory>().Where(i => i.AccountId == accountId).ToListAsync(ct);
}