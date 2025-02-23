using CritRDevEx.API.LoanAccount.CreateAccount;
using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.CreateAccount.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.CreateAccount;

public class EndpointTest
{
    [Fact]
    public void CreateAnAccountSucceeds()
    {
        CreateLoanAccount request = new(default);

        var (result, _) = CreateNewAccount(request);

        var okResult = Assert.IsType<Ok<Guid>>(result);
        Assert.IsType<Guid>(okResult.Value);
    }   
    
    [Fact]
    public void CreateAnAccountStartsStream()
    {
        CreateLoanAccount request = new(default);

        var (_, stream) = CreateNewAccount(request);

        Assert.IsType<LoanAccountCreated>(stream.Events.Single());
    }
}
