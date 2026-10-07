import { Injectable, computed, signal } from '@angular/core';
import { Customer } from './models';

const STORAGE_KEY = 'kadreebank.customer';

// La API o manda ningún token, así que se conserva el cliente devuelto por el login.
// Se usa sessionStorage para que la sesión termine al cerrar la pestaña.
@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly customerState = signal<Customer | null>(readStoredCustomer());

  readonly customer = this.customerState.asReadonly();
  readonly isLoggedIn = computed(() => this.customerState() !== null);

  start(customer: Customer): void {
    this.customerState.set(customer);
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(customer));
  }

  end(): void {
    this.customerState.set(null);
    sessionStorage.removeItem(STORAGE_KEY);
  }
}

function readStoredCustomer(): Customer | null {
  try {
    const stored = sessionStorage.getItem(STORAGE_KEY);
    return stored ? (JSON.parse(stored) as Customer) : null;
  } catch {
    return null;
  }
}
