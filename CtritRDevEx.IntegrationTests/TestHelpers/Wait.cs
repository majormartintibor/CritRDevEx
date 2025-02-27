using JasperFx.Core;

namespace CtritRDevEx.IntegrationTests.TestHelpers;

internal static class Wait
{
    internal static TimeSpan ForAsyncProjectionUpdateTime = 7.Seconds();    
    internal static TimeSpan Long = 30.Seconds();

    internal static int ForProcessorCycle = 3500;
}
