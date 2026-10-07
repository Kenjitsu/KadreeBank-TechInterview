import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards';
import { AccountDetail } from './pages/account-detail/account-detail';
import { Accounts } from './pages/accounts/accounts';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Reports } from './pages/reports/reports';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'accounts' },
  { path: 'login', component: Login, canActivate: [guestGuard], title: 'Ingresar · KadreeBank' },
  {
    path: 'register',
    component: Register,
    canActivate: [guestGuard],
    title: 'Registro · KadreeBank',
  },
  {
    path: 'accounts',
    component: Accounts,
    canActivate: [authGuard],
    title: 'Mis cuentas · KadreeBank',
  },
  {
    path: 'accounts/:id',
    component: AccountDetail,
    canActivate: [authGuard],
    title: 'Cuenta · KadreeBank',
  },
  // La API no tiene roles: los reportes son consultas generales, accesibles sin ingresar.
  { path: 'reports', component: Reports, title: 'Reportes · KadreeBank' },
  { path: '**', redirectTo: 'accounts' },
];
