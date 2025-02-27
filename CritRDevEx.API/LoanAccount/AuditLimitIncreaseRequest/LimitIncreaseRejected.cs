﻿namespace CritRDevEx.API.LoanAccount.AuditLimitIncreaseRequest;

public sealed record LimitIncreaseRejected(
        Guid LoanAccountId,
        DateTimeOffset RejectedAt
    ) : LoanAccountEvent;
