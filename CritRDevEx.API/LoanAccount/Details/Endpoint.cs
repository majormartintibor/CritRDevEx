using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace CritRDevEx.API.LoanAccount.Details;

public static class Endpoint
{
    //Levereging Postgres binary JSON format and directly streaming the JSONB via httpcontext
    //which is more efficient than serializing the object to a string and then writing it to the response
    public const string GetLoanAccountDetailEndpoint = "/api/loanAccount/detail/";
    [Tags(Tag.LoanAccount)]
    [WolverineGet(GetLoanAccountDetailEndpoint + "{loanAccountId:guid}")]
    public static Task GetLoanAccountDetail(
        [FromRoute] Guid loanAccountId,
        IQuerySession querySession,
        HttpContext context)
            =>  querySession.Json.WriteById<LoanAccountDetail>(loanAccountId, context);
}
