import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { errorMessage } from '../../core/error-message';
import { CustomerType } from '../../core/models';
import { SessionService } from '../../core/session.service';
import { CUSTOMER_TYPE_LABELS } from '../../shared/labels';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
})
export class Register {
  private readonly api = inject(ApiService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly customerTypes = Object.entries(CUSTOMER_TYPE_LABELS) as [
    CustomerType,
    string,
  ][];

  protected readonly form = inject(NonNullableFormBuilder).group({
    documentNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{5,20}$/)]],
    fullName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
    type: ['NaturalPerson' as CustomerType, Validators.required],
    pin: ['', [Validators.required, Validators.pattern(/^[0-9]{4}$/)]],
  });

  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const { fullName, ...rest } = this.form.getRawValue();

    // Tras registrarse se inicia la sesión directamente con el cliente creado.
    this.api.createCustomer({ ...rest, fullName: fullName.trim() }).subscribe({
      next: (customer) => {
        this.session.start(customer);
        this.router.navigate(['/accounts']);
      },
      error: (err) => {
        this.error.set(errorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
