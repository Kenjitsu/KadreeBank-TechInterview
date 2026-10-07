import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_URL } from './api.config';
import {
  Account,
  ApiResult,
  Balance,
  CreateCustomerRequest,
  Customer,
  LoginRequest,
  MonthlyStatement,
  MonthlyTransactionCountItem,
  OpenAccountRequest,
  OutOfCityWithdrawalItem,
  Transaction,
  TransactionRequest,
} from './models';

// Único punto de acceso a la API. Devuelve directamente el `data` del Result<T>;
// los errores llegan como HttpErrorResponse (ver errorMessage()).
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<Customer> {
    return this.unwrap(this.http.post<ApiResult<Customer>>(`${API_URL}/auth/login`, request));
  }

  createCustomer(request: CreateCustomerRequest): Observable<Customer> {
    return this.unwrap(this.http.post<ApiResult<Customer>>(`${API_URL}/customers`, request));
  }

  getCustomer(customerId: number): Observable<Customer> {
    return this.unwrap(this.http.get<ApiResult<Customer>>(`${API_URL}/customers/${customerId}`));
  }

  openAccount(request: OpenAccountRequest): Observable<Account> {
    return this.unwrap(this.http.post<ApiResult<Account>>(`${API_URL}/accounts`, request));
  }

  getAccount(accountId: number): Observable<Account> {
    return this.unwrap(this.http.get<ApiResult<Account>>(`${API_URL}/accounts/${accountId}`));
  }

  getBalance(accountId: number): Observable<Balance> {
    return this.unwrap(
      this.http.get<ApiResult<Balance>>(`${API_URL}/accounts/${accountId}/balance`),
    );
  }

  deposit(accountId: number, request: TransactionRequest): Observable<Transaction> {
    return this.unwrap(
      this.http.post<ApiResult<Transaction>>(`${API_URL}/accounts/${accountId}/deposits`, request),
    );
  }

  withdraw(accountId: number, request: TransactionRequest): Observable<Transaction> {
    return this.unwrap(
      this.http.post<ApiResult<Transaction>>(
        `${API_URL}/accounts/${accountId}/withdrawals`,
        request,
      ),
    );
  }

  getRecentTransactions(accountId: number, take: number): Observable<Transaction[]> {
    const params = new HttpParams().set('take', take);
    return this.unwrap(
      this.http.get<ApiResult<Transaction[]>>(`${API_URL}/accounts/${accountId}/transactions`, {
        params,
      }),
    );
  }

  getMonthlyStatement(
    accountId: number,
    year: number,
    month: number,
  ): Observable<MonthlyStatement> {
    return this.unwrap(
      this.http.get<ApiResult<MonthlyStatement>>(
        `${API_URL}/accounts/${accountId}/statements/${year}/${month}`,
      ),
    );
  }

  getMonthlyTransactionsReport(
    year: number,
    month: number,
  ): Observable<MonthlyTransactionCountItem[]> {
    const params = new HttpParams().set('year', year).set('month', month);
    return this.unwrap(
      this.http.get<ApiResult<MonthlyTransactionCountItem[]>>(
        `${API_URL}/reports/monthly-transactions`,
        { params },
      ),
    );
  }

  // year y month son opcionales: sin filtros la API suma todo el histórico.
  getOutOfCityWithdrawalsReport(
    year: number | null,
    month: number | null,
  ): Observable<OutOfCityWithdrawalItem[]> {
    let params = new HttpParams();
    if (year) params = params.set('year', year);
    if (month) params = params.set('month', month);
    return this.unwrap(
      this.http.get<ApiResult<OutOfCityWithdrawalItem[]>>(
        `${API_URL}/reports/out-of-city-withdrawals`,
        { params },
      ),
    );
  }

  private unwrap<T>(request: Observable<ApiResult<T>>): Observable<T> {
    return request.pipe(map((result) => result.data as T));
  }
}
