import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'utcDate' })
export class UtcDatePipe implements PipeTransform {
  transform(value: string | null | undefined): Date | null {
    if (!value) return null;
    const hasZone = /(Z|[+-]\d{2}:\d{2})$/.test(value);
    return new Date(hasZone ? value : `${value}Z`);
  }
}
