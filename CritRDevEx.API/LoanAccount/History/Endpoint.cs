using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.History;

public static class Endpoint
{
    public const string GetAccountHistoryEnpoint = "/api/loanAccount/history/";

    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetAccountHistoryEnpoint + "{loanAccountId:guid}")]    
    public static Task GetInspection([FromRoute] Guid LoanAccountId, IQuerySession querySession, CancellationToken ct)
        => querySession.Query<LoanAccountHistory>().Where(i => i.LoanAccountId == LoanAccountId).ToListAsync(ct);
}