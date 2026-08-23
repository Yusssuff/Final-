import {
  Component,
  inject
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Router
} from '@angular/router';

import {
  AuthService
} from './auth.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './auth.html',
  styleUrl: './auth.css'
})
export class AuthComponent {

  private readonly fb =
    inject(FormBuilder);

  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  isLogin = true;

  isLoading = false;

  errorMessage = '';

  successMessage = '';

  readonly loginForm =
    this.fb.group({
      username: [
        '',
        Validators.required
      ],

      password: [
        '',
        Validators.required
      ]
    });

  readonly registerForm =
    this.fb.group({
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(3)
        ]
      ],

      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ],

      confirmPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ]
    });

  switchToLogin(): void {

    if (this.isLogin) {
      return;
    }

    this.isLogin = true;

    this.clearMessages();
  }

  switchToRegister(): void {

    if (!this.isLogin) {
      return;
    }

    this.isLogin = false;

    this.clearMessages();
  }

  login(): void {

    if (this.loginForm.invalid) {

      this.loginForm.markAllAsTouched();

      return;
    }

    this.isLoading = true;

    this.clearMessages();

    const username =
      this.loginForm.get('username')?.value ?? '';

    const password =
      this.loginForm.get('password')?.value ?? '';

    this.authService
      .login({
        username,
        password
      })
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          this.successMessage =
            response.message ||
            `Welcome ${response.user.username}.`;

          /*
           * AuthService has already stored
           * the JWT in localStorage.
           *
           * Now go to Products.
           */
          void this.router
            .navigate(['/products']);
        },

        error: (error: unknown) => {

          this.isLoading = false;

          const httpError =
            error as {
              error?: {
                message?: string;
              };
            };

          this.errorMessage =
            httpError.error?.message ??
            'Unable to sign in.';
        }

      });
  }

  register(): void {

    if (this.registerForm.invalid) {

      this.registerForm.markAllAsTouched();

      return;
    }

    const username =
      this.registerForm
        .get('username')?.value ?? '';

    const password =
      this.registerForm
        .get('password')?.value ?? '';

    const confirmPassword =
      this.registerForm
        .get('confirmPassword')?.value ?? '';

    if (
      password !==
      confirmPassword
    ) {

      this.errorMessage =
        'Passwords do not match.';

      return;
    }

    this.isLoading = true;

    this.clearMessages();

    this.authService
      .register({
        username,
        password
      })
      .subscribe({

        next: (response) => {

          this.isLoading = false;

          this.registerForm.reset();

          this.successMessage =
            response.message ||
            'Account created successfully.';

          this.isLogin = true;
        },

        error: (error: unknown) => {

          this.isLoading = false;

          const httpError =
            error as {
              error?: {
                message?: string;
              };
            };

          this.errorMessage =
            httpError.error?.message ??
            'Unable to create the account.';
        }

      });
  }

  private clearMessages(): void {

    this.errorMessage = '';

    this.successMessage = '';
  }
}
