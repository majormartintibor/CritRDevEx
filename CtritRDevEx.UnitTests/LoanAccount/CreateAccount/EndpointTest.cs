using Microsoft.AspNetCore.Http.HttpResults;
using static CritRDevEx.API.LoanAccount.CreateAccount.Endpoint;

namespace CtritRDevEx.UnitTests.LoanAccount.CreateAccount;

public class EndpointTest
{
    [Fact]
    public void DebtorWithNoExistingAccountCanCreateAnAccount()
    {
        CreateLoanAccount request = new(default);

        var (result, _) = CreateNewAccount(request, false);

        var okResult = Assert.IsType<Ok<Guid>>(result);
        Assert.IsType<Guid>(okResult.Value);
    }

    [Fact]
    public void DebtorWithExistingAccountCannotCreateAnAccount()
    {
        CreateLoanAccount request = new(default);        

        Assert.Throws<InvalidOperationException>(() => CreateNewAccount(request, true));        
    }
}
