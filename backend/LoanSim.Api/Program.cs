using LoanSim.Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ILoanService, LoanService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoanRequestValidator>();

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p =>
        p.AllowAnyHeader().AllowAnyMethod().WithOrigins(
            builder.Configuration["ALLOW_ORIGINS"]?.Split(';', StringSplitOptions.RemoveEmptyEntries)
             ?? new[] { "http://localhost:5173", "https://localhost:5173", "https://victorious-river-0d8cb131e.1.azurestaticapps.net" }
        )
    )
);

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapPost("/api/loans/calc", (LoanRequest req, ILoanService svc, TelemetryClient telemetry) => {
    telemetry.TrackEvent("LoanCalculation", new Dictionary<string, string> {
        { "Principal", req.Principal.ToString() },
        { "Rate", req.AnnualRatePercent.ToString() },
        { "Term", req.TermMonths.ToString() }
    });

    var result = svc.Calculate(req);
    return Results.Ok(result);
})
.WithName("CalcLoan");


app.Run();

public sealed class LoanRequestValidator : AbstractValidator<LoanRequest> {
    public LoanRequestValidator() {
        RuleFor(x => x.Principal).GreaterThan(0);
        RuleFor(x => x.AnnualRatePercent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TermMonths).InclusiveBetween(1, 1200);
        RuleFor(x => x.ExtraMonthly).GreaterThanOrEqualTo(0).When(x => x.ExtraMonthly.HasValue);
    }
}