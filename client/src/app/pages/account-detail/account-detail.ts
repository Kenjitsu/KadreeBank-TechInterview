import { LowerCasePipe } from '@angular/common';
import { Component, OnInit, computed, inject, input, numberAttribute, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { errorMessage } from '../../core/error-message';
import { Account, MonthlyStatement, Transaction, TransactionType } from '../../core/models';
import { SessionService } from '../../core/session.service';
import {
  ACCOUNT_TYPE_LABELS,
  CITIES,
  MONTHS,
  TRANSACTION_TYPE_LABELS,
  lastYears,
} from '../../shared/labels';
import { MoneyPipe } from '../../shared/money.pipe';
import { TransactionsTable } from '../../shared/transactions-table';

@Component({
  selector: 'app-account-detail',
  imports: [ReactiveFormsModule, RouterLink, LowerCasePipe, MoneyPipe, TransactionsTable],
  templateUrl: './account-detail.html',
})
export class AccountDetail implements OnInit {
  private readonly api = inject(ApiService);
  private readonly session = inject(SessionService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly money = new MoneyPipe();

  // Parámetro de ruta :id (withComponentInputBinding).
  readonly id = input.required({ transform: numberAttribute });

  protected readonly accountTypeLabels = ACCOUNT_TYPE_LABELS;
  protected readonly months = MONTHS;
  protected readonly years = lastYears(5);

  // Cuenta y saldo
  protected readonly account = signal<Account | null>(null);
  protected readonly balance = signal<number | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly refreshingBalance = signal(false);

  // Si la cuenta se abrió con una ciudad fuera de la lista (p. ej. desde Swagger), se agrega para poder preseleccionarla.
  protected readonly cityOptions = computed(() => {
    const city = this.account()?.city;
    return city && !CITIES.includes(city) ? [city, ...CITIES] : CITIES;
  });

  // Consignación / retiro
  protected readonly operation = signal<TransactionType>('Deposit');
  protected readonly operationForm = this.fb.group({
    amount: this.fb.control<number | null>(null, [
      Validators.required,
      Validators.min(0.01),
      Validators.pattern(/^\d+(\.\d{1,2})?$/),
    ]),
    city: ['', Validators.required],
  });
  protected readonly submitting = signal(false);
  protected readonly operationError = signal<string | null>(null);
  protected readonly operationSuccess = signal<string | null>(null);

  // Movimientos recientes
  protected readonly takeOptions = [5, 10, 20, 50];
  protected readonly take = signal(10);
  protected readonly transactions = signal<Transaction[]>([]);
  protected readonly transactionsError = signal<string | null>(null);

  // Extracto mensual
  protected readonly statementForm = this.fb.group({
    year: [new Date().getFullYear()],
    month: [new Date().getMonth() + 1],
  });
  protected readonly statement = signal<MonthlyStatement | null>(null);
  protected readonly statementError = signal<string | null>(null);
  protected readonly loadingStatement = signal(false);

  ngOnInit(): void {
    this.api.getAccount(this.id()).subscribe({
      next: (account) => {
        // Login simulado: solo se muestran cuentas del cliente en sesión.
        if (account.customerId !== this.session.customer()?.id) {
          this.loadError.set('Esta cuenta no pertenece al cliente actual.');
          return;
        }
        this.account.set(account);
        this.balance.set(account.balance);
        this.operationForm.controls.city.setValue(account.city);
        this.loadTransactions();
        this.loadStatement();
      },
      error: (err) => this.loadError.set(errorMessage(err)),
    });
  }

  protected refreshBalance(): void {
    this.refreshingBalance.set(true);
    this.api.getBalance(this.id()).subscribe({
      next: (result) => {
        this.balance.set(result.balance);
        this.refreshingBalance.set(false);
      },
      error: (err) => {
        this.operationError.set(errorMessage(err));
        this.refreshingBalance.set(false);
      },
    });
  }

  protected selectOperation(type: TransactionType): void {
    this.operation.set(type);
    this.operationError.set(null);
    this.operationSuccess.set(null);
  }

  protected submitOperation(): void {
    if (this.operationForm.invalid) {
      this.operationForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.operationError.set(null);
    this.operationSuccess.set(null);

    const { amount, city } = this.operationForm.getRawValue();
    const request = { amount: Number(amount), city };
    const isDeposit = this.operation() === 'Deposit';
    const call = isDeposit
      ? this.api.deposit(this.id(), request)
      : this.api.withdraw(this.id(), request);

    call.subscribe({
      next: (transaction) => {
        this.operationSuccess.set(
          `${TRANSACTION_TYPE_LABELS[transaction.type]} por ${this.money.transform(transaction.amount)} realizada.`,
        );
        this.operationForm.reset({ amount: null, city: this.account()?.city ?? '' });
        this.submitting.set(false);
        this.refreshBalance();
        this.loadTransactions();
        if (this.statement()) this.loadStatement();
      },
      error: (err) => {
        this.operationError.set(errorMessage(err));
        this.submitting.set(false);
      },
    });
  }

  protected changeTake(value: string): void {
    this.take.set(Number(value));
    this.loadTransactions();
  }

  protected loadTransactions(): void {
    this.transactionsError.set(null);
    this.api.getRecentTransactions(this.id(), this.take()).subscribe({
      next: (transactions) => this.transactions.set(transactions),
      error: (err) => this.transactionsError.set(errorMessage(err)),
    });
  }

  protected loadStatement(): void {
    const { year, month } = this.statementForm.getRawValue();
    this.loadingStatement.set(true);
    this.statementError.set(null);

    this.api.getMonthlyStatement(this.id(), Number(year), Number(month)).subscribe({
      next: (statement) => {
        this.statement.set(statement);
        this.loadingStatement.set(false);
      },
      error: (err) => {
        this.statementError.set(errorMessage(err));
        this.loadingStatement.set(false);
      },
    });
  }
}
