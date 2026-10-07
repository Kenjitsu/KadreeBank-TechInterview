export interface ApiResult<T> {
  isSuccess: boolean;
  statusCode: number;
  message: string | null;
  data: T | null;
  error: ApiError | null;
}

export interface ApiError {
  code: string;
  description: string | null;
}

export type CustomerType = 'NaturalPerson' | 'Company';
export type AccountType = 'Savings' | 'Checking';
export type TransactionType = 'Deposit' | 'Withdrawal';

export interface Account {
  id: number;
  accountNumber: string;
  customerId: number;
  type: AccountType;
  city: string;
  balance: number;
  createdAt: string;
}

export interface Customer {
  id: number;
  documentNumber: string;
  fullName: string;
  type: CustomerType;
  createdAt: string;
  accounts: Account[];
}

export interface Balance {
  accountId: number;
  accountNumber: string;
  balance: number;
}

export interface Transaction {
  id: number;
  accountId: number;
  type: TransactionType;
  amount: number;
  city: string;
  balanceAfter: number;
  createdAt: string;
}

export interface MonthlyStatement {
  accountId: number;
  accountNumber: string;
  year: number;
  month: number;
  openingBalance: number;
  totalDeposits: number;
  totalWithdrawals: number;
  closingBalance: number;
  transactions: Transaction[];
}

export interface MonthlyTransactionCountItem {
  customerId: number;
  documentNumber: string;
  fullName: string;
  transactionCount: number;
}

export interface OutOfCityWithdrawalItem {
  customerId: number;
  documentNumber: string;
  fullName: string;
  withdrawalCount: number;
  totalWithdrawn: number;
}

// Requests

export interface LoginRequest {
  documentNumber: string;
  pin: string;
}

export interface CreateCustomerRequest {
  documentNumber: string;
  fullName: string;
  type: CustomerType;
  pin: string;
}

export interface OpenAccountRequest {
  customerId: number;
  type: AccountType;
  city: string;
}

export interface TransactionRequest {
  amount: number;
  city: string;
}
