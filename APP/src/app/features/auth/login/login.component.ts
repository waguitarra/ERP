import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private router = inject(Router);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      await this.authService.login(this.loginForm.value);
    } catch (error: any) {
      this.errorMessage.set(error.message || 'Erro ao fazer login. Verifique suas credenciais.');
    } finally {
      this.isLoading.set(false);
    }
  }

  fillDemoCredentials(): void {
    this.loginForm.patchValue({
      email: 'admin@WMS.com',
      password: 'admin@123456'
    });
  }
}
