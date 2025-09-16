import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { LoginUserRequest } from '../../../core/auth/dto/login-user-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  showPassword = false;
  errorMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (!this.email || !this.password) return;
  
    // Debug: proveri šta korisnik unosi
    console.log('Email unet:', this.email);
    console.log('Password unet:', this.password);
  
    const request: LoginUserRequest = { email: this.email, password: this.password };
    this.authService.login(request).subscribe({
      next: () => this.router.navigate(['/questions']),
      error: (err) => {
        console.error('Login error', err); // vidi ceo odgovor
        this.errorMessage = err?.error?.message || 'Login failed. Check your credentials.';
      }
    });
  }
  
}
