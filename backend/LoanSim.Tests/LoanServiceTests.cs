using FluentAssertions;
using LoanSim.Domain;

namespace LoanSim.Tests;

public class LoanServiceTests {
    [Fact]
    public void Calc_FixedRate_NoExtra_PayOff() {
        var svc = new LoanService();
        var req = new LoanRequest(Principal: 300_00m, AnnualRatePercent: 6.0m, TermMonths: 360);
        var res = svc.Calculate(req);

        res.Schedule.Should().NotBeEmpty();
        res.Schedule.Last().RemainingBalance.Should().Be(0);
        res.MonthlyPayment.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Calc_ZeroRate_SplitsEvenly() {
        var svc = new LoanService();
        var req = new LoanRequest(Principal: 1200m, AnnualRatePercent: 0m, TermMonths: 12);
        var res = svc.Calculate(req);

        res.MonthlyPayment.Should().Be(100m);
        res.TotalInterest.Should().Be(0m);
        res.Schedule.Should().NotBeEmpty();
        res.Schedule.Last().RemainingBalance.Should().Be(0);
        res.MonthlyPayment.Should().BeGreaterThan(0);
    }
}
