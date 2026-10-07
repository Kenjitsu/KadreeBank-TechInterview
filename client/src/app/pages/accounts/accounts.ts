import { LowerCasePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { errorMessage } from '../../core/error-message';
import { SessionService } from '../../core/session.service';
import {
  ACCOUNT_TYPE_BY_CUSTOMER,
  ACCOUNT_TYPE_LABELS,
  CITIES,
  CUSTOMER_TYPE_LABELS,
} from '../../shared/labels';
import { MoneyPipe } from '../../shared/money.pipe';

@Component({
  selector: 'app-accounts',
  imports: [ReactiveFormsModule, RouterLink, LowerCasePipe, MoneyPipe],
  templateUrl: './accounts.html',
})
export class Accounts implements OnInit {
  private readonly api = inject(ApiService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly customerTypeLabels = CUSTOMER_TYPE_LABELS;
  protected readonly accountTypeLabels = ACCOUNT_TYPE_LABELS;
  protected readonly cities = CITIES;

  protected readonly customer = this.session.customer;
  protected readonly totalBalance = computed(
    () => this.customer()?.accounts.reduce((sum, account) => sum + account.balance, 0) ?? 0,
  );
  // El tipo de cuenta lo determina el tipo de cliente (regla de negocio de la API).
  protected readonly newAccountType = computed(() => {
    const customer = this.customer();
    return customer ? ACCOUNT_TYPE_BY_CUSTOMER[customer.type] : null;
  });

  protected readonly loadError = signal<string | null>(null);

  protected readonly openForm = inject(NonNullableFormBuilder).group({
    city: ['', Validators.required],
  });
  protected readonly opening = signal(false);
  protected readonly openError = signal<string | null>(null);
  protected readonly openSuccess = signal<string | null>(null);

  ngOnInit(): void {
    this.refreshCustomer();
  }

  // Los saldos guardados en la sesión pueden estar desactualizados: se recarga el cliente.
  private refreshCustomer(): void {
    const customer = this.customer();
    if (!customer) return;

    this.api.getCustomer(customer.id).subscribe({
      next: (fresh) => this.session.start(fresh),
      error: (err) => {
        if (err instanceof HttpErrorResponse && err.status === 404) {
          this.session.end();
          this.router.navigate(['/login']);
          return;
        }
        this.loadError.set(errorMessage(err));
      },
    });
  }

  protected openAccount(): void {
    const customer = this.customer();
    const type = this.newAccountType();
    if (!customer || !type) return;

    if (this.openForm.invalid) {
      this.openForm.markAllAsTouched();
      return;
    }

    this.opening.set(true);
    this.openError.set(null);
    this.openSuccess.set(null);

    const city = this.openForm.controls.city.value;

    this.api.openAccount({ customerId: customer.id, type, city }).subscribe({
      next: (account) => {
        this.openSuccess.set(`Cuenta ${account.accountNumber} abierta en ${account.city}.`);
        this.openForm.reset();
        this.opening.set(false);
        this.refreshCustomer();
      },
      error: (err) => {
        this.openError.set(errorMessage(err));
        this.opening.set(false);
      },
    });
  }
}
