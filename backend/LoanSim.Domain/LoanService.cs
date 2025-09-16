using System.Globalization;

namespace LoanSim.Domain;

public interface ILoanService {
    LoanResult Calculate(LoanRequest req, DateOnly? startDate = null);
}

public sealed class LoanService : ILoanService {
    public LoanResult Calculate(LoanRequest req, DateOnly? startDate = null) {
        if (req.Principal <= 0 || req.AnnualRatePercent < 0 || req.TermMonths <= 0)
            throw new ArgumentException("Invalid load request");

        var start = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var monthlyRate = (decimal)((double)req.AnnualRatePercent / 100.0 / 12.0);
        var extra = req.ExtraMonthly.GetValueOrDefault(0m);

        decimal basePayment;
        if (monthlyRate == 0) {
            basePayment = decimal.Round(req.Principal / req.TermMonths, 2, MidpointRounding.AwayFromZero);
        } else {
            // P * r * (1+r)^n / ((1+r)^n - 1)
            var r = monthlyRate;
            var n = req.TermMonths;
            var onePlus = (decimal)Math.Pow((double)(1 + r), n);
            basePayment = req.Principal * r * onePlus / (onePlus - 1);
            basePayment = decimal.Round(basePayment, 2, MidpointRounding.AwayFromZero);
        }

        var schedule = new List<PaymentRow>(req.TermMonths + 12);
        var balance = req.Principal;
        var month = 0;
        var totalInterest = 0m;

        while (balance > 0 && month < req.TermMonths + 600) {
            month++;
            var date = start.AddMonths(month);
            var interest = decimal.Round(balance * monthlyRate, 2, MidpointRounding.AwayFromZero);

            var payment = basePayment + extra;
            if (payment > balance + interest) payment = balance + interest;

            var principalPaid = payment - interest;
            balance = decimal.Round(balance - principalPaid, 2, MidpointRounding.AwayFromZero);
            if (balance < 0) balance = 0;

            totalInterest += interest;

            schedule.Add(new PaymentRow(
                Month: month,
                Date: date,
                Payment: payment,
                PrincipalPaid: principalPaid,
                InterestPaid: interest,
                RemainingBalance: balance
            ));

            if (balance == 0) break;
        }

        var totalPaid = schedule.Sum(x => x.Payment);
        var payoffDate = schedule.Last().Date;

        return new LoanResult(
            MonthlyRate: monthlyRate,
            MonthlyPayment: basePayment,
            TotalInterest: decimal.Round(totalInterest, 2),
            TotalPaid: decimal.Round(totalPaid, 2),
            PayoffDate: payoffDate,
            Schedule: schedule
        );
    }
}