import { formatCurrency } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'money' })
export class MoneyPipe implements PipeTransform {
  transform(value: number | null | undefined): string {
    if (value === null || value === undefined) return '';
    return formatCurrency(value, 'es-CO', '$', 'COP', '1.0-2');
  }
}
