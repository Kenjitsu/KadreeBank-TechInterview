import { AccountType, CustomerType, TransactionType } from '../core/models';

// Textos en español para los enums de la API.

export const CUSTOMER_TYPE_LABELS: Record<CustomerType, string> = {
  NaturalPerson: 'Persona natural',
  Company: 'Empresa',
};

export const ACCOUNT_TYPE_LABELS: Record<AccountType, string> = {
  Savings: 'Ahorros',
  Checking: 'Corriente',
};

export const TRANSACTION_TYPE_LABELS: Record<TransactionType, string> = {
  Deposit: 'Consignación',
  Withdrawal: 'Retiro',
};

export const ACCOUNT_TYPE_BY_CUSTOMER: Record<CustomerType, AccountType> = {
  NaturalPerson: 'Savings',
  Company: 'Checking',
};

export const MONTHS = [
  'Enero',
  'Febrero',
  'Marzo',
  'Abril',
  'Mayo',
  'Junio',
  'Julio',
  'Agosto',
  'Septiembre',
  'Octubre',
  'Noviembre',
  'Diciembre',
];

// Sin tildes, igual que los datos sembrados: SQL Server distingue tildes al comparar ciudades.
export const CITIES = [
  'Barranquilla',
  'Bogota',
  'Bucaramanga',
  'Cali',
  'Cartagena',
  'Cucuta',
  'Ibague',
  'Manizales',
  'Medellin',
  'Pereira',
  'Santa Marta',
  'Villavicencio',
];

export function lastYears(count: number): number[] {
  const current = new Date().getFullYear();
  return Array.from({ length: count }, (_, i) => current - i);
}
