import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { catchError, switchMap, of } from 'rxjs';
import { LoaderService } from '../../../common/ui/loader/loader.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;
  photoFile: File | null = null;
  photoPreview: string | ArrayBuffer | null = null;
  isDragging = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private loaderService: LoaderService
  ) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      gender: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      state: [''],
      city: [''],
      address: ['']
    }, { validators: this.passwordsMatch });
  }

  passwordsMatch(group: FormGroup) {
    const pass = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pass === confirm ? null : { passwordMismatch: true };
  }

  get name() { return this.registerForm.get('name'); }
  get lastname() { return this.registerForm.get('lastname'); }
  get gender() { return this.registerForm.get('gender'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }

  onDragOver(event: DragEvent) { event.preventDefault(); this.isDragging = true; }
  onDragLeave(event: DragEvent) { event.preventDefault(); this.isDragging = false; }
  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    if (event.dataTransfer?.files?.length) this.setFile(event.dataTransfer.files[0]);
  }
  onFileSelected(event: any) { if (event.target.files?.[0]) this.setFile(event.target.files[0]); }
  private setFile(file: File) {
    this.photoFile = file;
    const reader = new FileReader();
    reader.onload = () => (this.photoPreview = reader.result);
    reader.readAsDataURL(file);
  }

  onSubmit(): void {
    this.registerForm.markAllAsTouched();
    if (this.registerForm.invalid) return;
  
    const formValue = this.registerForm.value;
  
    const request = {
      name: formValue.name,
      lastname: formValue.lastname,
      email: formValue.email,
      password: formValue.password,
      gender: formValue.gender,
      state: formValue.state,
      city: formValue.city,
      address: formValue.address
    };
  
    this.loaderService.show();
    this.authService.register(request)
      .pipe(
        switchMap(() => this.authService.login({ email: request.email, password: request.password })),
        switchMap(loginRes => {
          localStorage.setItem('token', loginRes.AccessToken);
  
          return this.photoFile ? this.userService.uploadPhoto(this.photoFile) : of(null);
        }),
        catchError(err => {
          console.error('Registration or photo upload failed:', err);
          return of(null); 
        })
      )
      .subscribe({
        next: () => {
          console.log('Registration successful');
          this.router.navigate(['/questions']);
          this.loaderService.hide();
        },
        error: () => {
          console.error('Something went wrong')
          this.loaderService.hide();
        }
      });
  }
  
  
}
