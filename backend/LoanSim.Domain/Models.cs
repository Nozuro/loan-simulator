namespace LoanSim.Domain;

public sealed record LoanRequest(
    decimal Principal,
    decimal AnnualRatePercent,
    int TermMonths,
    decimal? ExtraMonthly = null
);

public sealed record PaymentRow(
    int Month,
    DateOnly Date,
    decimal Payment,
    decimal PrincipalPaid,
    decimal InterestPaid,
    decimal RemainingBalance
);

public sealed record LoanResult(
    decimal MonthlyRate,
    decimal MonthlyPayment,
    decimal TotalInterest,
    decimal TotalPaid,
    DateOnly PayoffDate,
    IReadOnlyList<PaymentRow> Schedule
);