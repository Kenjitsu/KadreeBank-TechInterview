import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SessionService } from './session.service';

// rutas privadas, requieren haber ingresado con su usuario a la app.
export const authGuard: CanActivateFn = () =>
  inject(SessionService).isLoggedIn() || inject(Router).createUrlTree(['/login']);

// si ya hay sesión, se va directo a las cuentas.
export const guestGuard: CanActivateFn = () =>
  !inject(SessionService).isLoggedIn() || inject(Router).createUrlTree(['/accounts']);
