import { Component, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { errorMessage } from '../../core/error-message';
import { MonthlyTransactionCountItem, OutOfCityWithdrawalItem } from '../../core/models';
import { MONTHS, lastYears } from '../../shared/labels';
import { MoneyPipe } from '../../shared/money.pipe';

@Component({
  selector: 'app-reports',
  imports: [ReactiveFormsModule, MoneyPipe],
  templateUrl: './reports.html',
})
export class Reports implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly months = MONTHS;
  protected readonly years = lastYears(5);

  // Reporte 1: clientes por número de transacciones en un mes (año y mes obligatorios).
  protected readonly monthlyForm = this.fb.group({
    year: [new Date().getFullYear()],
    month: [new Date().getMonth() + 1],
  });
  protected readonly monthlyItems = signal<MonthlyTransactionCountItem[] | null>(null);
  protected readonly monthlyError = signal<string | null>(null);
  protected readonly loadingMonthly = signal(false);

  // Reporte 2: retiros fuera de la ciudad de origen > $1.000.000 (año y mes opcionales).
  protected readonly outOfCityForm = this.fb.group({
    year: this.fb.control<number | null>(null),
    month: this.fb.control<number | null>({ value: null, disabled: true }),
  });
  protected readonly outOfCityItems = signal<OutOfCityWithdrawalItem[] | null>(null);
  protected readonly outOfCityError = signal<string | null>(null);
  protected readonly loadingOutOfCity = signal(false);

  constructor() {
    // La API exige el año para filtrar por mes: sin año, el mes se deshabilita.
    this.outOfCityForm.controls.year.valueChanges.pipe(takeUntilDestroyed()).subscribe((year) => {
      const month = this.outOfCityForm.controls.month;
      if (year) {
        month.enable();
      } else {
        month.reset(null);
        month.disable();
      }
    });
  }

  ngOnInit(): void {
    this.loadMonthly();
    this.loadOutOfCity();
  }

  protected loadMonthly(): void {
    const { year, month } = this.monthlyForm.getRawValue();
    this.loadingMonthly.set(true);
    this.monthlyError.set(null);

    this.api.getMonthlyTransactionsReport(Number(year), Number(month)).subscribe({
      next: (items) => {
        this.monthlyItems.set(items);
        this.loadingMonthly.set(false);
      },
      error: (err) => {
        this.monthlyError.set(errorMessage(err));
        this.loadingMonthly.set(false);
      },
    });
  }

  protected loadOutOfCity(): void {
    const { year, month } = this.outOfCityForm.getRawValue();
    this.loadingOutOfCity.set(true);
    this.outOfCityError.set(null);

    this.api.getOutOfCityWithdrawalsReport(year, year ? month : null).subscribe({
      next: (items) => {
        this.outOfCityItems.set(items);
        this.loadingOutOfCity.set(false);
      },
      error: (err) => {
        this.outOfCityError.set(errorMessage(err));
        this.loadingOutOfCity.set(false);
      },
    });
  }
}
