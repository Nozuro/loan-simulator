import { useState } from "react";
import { calcLoan, type LoanResult } from "./api";

export default function App() {
  const [principal, setPrincipal] = useState(350000);
  const [rate, setRate] = useState(6.25);
  const [months, setMonths] = useState(360);
  const [extra, setExtra] = useState(0);
  const [result, setResult] = useState<LoanResult | null>(null);
  const [busy, setBusy] = useState(false);

  const onSubmit = async (e: React.FormEvent) => {
    console.log("asd");
    e.preventDefault();
    setBusy(true);
    try {
      const res = await calcLoan({
        principal,
        annualRatePercent: rate,
        termMonths: months,
        extraMonthly: extra || undefined,
      });
      setResult(res);
    } finally {
      setBusy(false);
    }
  };

  return (
    <main style={{ maxWidth: 920, margin: "2rem auto", padding: 16 }}>
      <h1>Loan Simulator</h1>
      <form onSubmit={onSubmit} style={{ display: "grid", gap: 12, gridTemplateColumns: "repeat(4, 1fr)" }}>
        <label>Principal
          <input type="number" value={principal} onChange={e => setPrincipal(+e.target.value)} />
        </label>
        <label> Rate % (APR)
          <input type="number" step="0.01" value={rate} onChange={e => setRate(+e.target.value)} />
        </label>
        <label>Term (months)
          <input type="number" value={months} onChange={e => setMonths(+e.target.value)} />
        </label>
        <label>Extra / month
          <input type="number" value={extra} onChange={e => setExtra(+e.target.value)} />
        </label>
        <button disabled={busy} style={{ gridColumn: "1 / -1" }}> {busy ? "Calculating..." : "Calculate"}</button>
      </form>

      {result && (
        <>
          <section style={{marginTop : 24}}>
            <h2>Totals</h2>
            <ul>
              <li>Monthly Payment (base): ${result.monthlyPayment.toFixed(2)}</li>
              <li>Total Interest: ${result.totalInterest.toFixed(2)}</li>
              <li>Total Paid: ${result.totalPaid.toFixed(2)}</li>
              <li>Payoff Date: {result.payoffDate}</li>
            </ul>
          </section>

          <section style={{marginTop:24}}>
            <h2>First 12 Payments</h2>
            <table>
              <thead>
                <tr><th>Month</th><th>Date</th><th>Payment</th><th>Principal</th><th>Interest</th></tr>
              </thead>
              <tbody>
                {result.schedule.slice(0,12).map(r=> (
                  <tr key ={r.month}>
                    <td>{r.month}</td>
                    <td>{r.date}</td>
                    <td>{r.payment.toFixed(2)}</td>
                    <td>{r.principalPaid.toFixed(2)}</td>
                    <td>{r.interestPaid.toFixed(2)}</td>
                    <td>{r.remainingBalance.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </section>
        </>
      )}
    </main>
  )
}