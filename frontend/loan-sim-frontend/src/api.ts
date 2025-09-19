import axios from "axios";

const api = axios.create({
    // baseURL: import.meta.env.API_BASE,
    baseURL: "https://app-loansim-api2-f6eybfd6g8gqe9hc.westus3-01.azurewebsites.net/swagger/index.html",
    timeout: 10000,
});

export type LoanRequest = {
    principal: number;
    annualRatePercent: number;
    termMonths: number;
    extraMonthly?: number;
};

export type LoanResult = {
    monthlyRate: number;
    monthlyPayment: number;
    totalInterest: number;
    totalPaid: number;
    payoffDate: string;
    schedule: Array<{
        month: number;
        date: string;
        payment: number;
        principalPaid: number;
        interestPaid: number;
        remainingBalance: number;
    }>
}

export async function calcLoan(req: LoanRequest){
    const{data} = await api.post<LoanResult>("/api/loans/calc", req);
    return data;
}