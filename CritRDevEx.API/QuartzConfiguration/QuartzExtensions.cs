using CritRDevEx.API.LoanAccount.LimitIncreaseProcessor;
using Quartz;

namespace CritRDevEx.API.QuartzConfiguration;

public static class QuartzExtensions
{
    public static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {            
            var jobKey = new JobKey(nameof(PendingLimitIncreaseRequestProcessor));
            q.AddJob<PendingLimitIncreaseRequestProcessor>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(PendingLimitIncreaseRequestProcessor)}-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(3)
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
