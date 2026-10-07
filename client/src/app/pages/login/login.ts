import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { errorMessage } from '../../core/error-message';
import { SessionService } from '../../core/session.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
})
export class Login {
  private readonly api = inject(ApiService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly form = inject(NonNullableFormBuilder).group({
    documentNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{5,20}$/)]],
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

    this.api.login(this.form.getRawValue()).subscribe({
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
