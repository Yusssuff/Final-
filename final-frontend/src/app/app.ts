import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html'
})
export class AppComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  get isLoggedIn(): boolean {
    return this.auth.isLoggedIn();
  }

  get isAuthPage(): boolean {
    return this.router.url.startsWith('/auth');
  }

  get username(): string | null {
    return this.auth.getUser()?.username ?? null;
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigate(['/auth']);
  }
}
