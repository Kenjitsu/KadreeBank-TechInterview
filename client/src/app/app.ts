import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SessionService } from './core/session.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
})
export class App {
  private readonly router = inject(Router);
  protected readonly session = inject(SessionService);

  protected logout(): void {
    this.session.end();
    this.router.navigate(['/login']);
  }
}
