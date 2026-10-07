import { DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { Transaction } from '../core/models';
import { TRANSACTION_TYPE_LABELS } from './labels';
import { MoneyPipe } from './money.pipe';
import { UtcDatePipe } from './utc-date.pipe';

@Component({
  selector: 'app-transactions-table',
  imports: [DatePipe, MoneyPipe, UtcDatePipe],
  template: `
    @if (transactions().length === 0) {
      <p class="empty">{{ emptyMessage() }}</p>
    } @else {
      <div class="overflow-x-auto">
        <table class="table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Tipo</th>
              <th>Ciudad</th>
              <th class="text-right">Monto</th>
              <th class="text-right">Saldo</th>
            </tr>
          </thead>
          <tbody>
            @for (transaction of transactions(); track transaction.id) {
              <tr>
                <td class="whitespace-nowrap text-slate-500">
                  {{ transaction.createdAt | utcDate | date: 'd MMM y, h:mm a' }}
                </td>
                <td>
                  <span
                    class="badge"
                    [class]="
                      transaction.type === 'Deposit'
                        ? 'bg-emerald-50 text-emerald-700'
                        : 'bg-rose-50 text-rose-700'
                    "
                  >
                    {{ typeLabels[transaction.type] }}
                  </span>
                </td>
                <td class="text-slate-600">{{ transaction.city }}</td>
                <td
                  class="text-right font-medium whitespace-nowrap"
                  [class]="transaction.type === 'Deposit' ? 'text-emerald-600' : 'text-rose-600'"
                >
                  {{ transaction.type === 'Deposit' ? '+' : '−' }}{{ transaction.amount | money }}
                </td>
                <td class="text-right whitespace-nowrap text-slate-700">
                  {{ transaction.balanceAfter | money }}
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }
  `,
})
export class TransactionsTable {
  readonly transactions = input.required<Transaction[]>();
  readonly emptyMessage = input('No hay movimientos.');

  protected readonly typeLabels = TRANSACTION_TYPE_LABELS;
}
