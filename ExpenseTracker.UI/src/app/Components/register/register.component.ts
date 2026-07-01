import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterRequest } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  errorMessage: string = '';
  showPassword = false;
  showConfirmPassword = false;
  registerForm!: FormGroup;

  private formBuilder = inject(FormBuilder); 
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      Name: ['', [Validators.required, Validators.minLength(3)]],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6)]],
      ConfirmPassword: ['', [Validators.required, Validators.minLength(6)]]
    },
    {
      validators: RegisterComponent.passwordMatch
    });
  }


  static passwordMatch(control: AbstractControl): ValidationErrors | null {
    const password = control.get('Password')?.value;
    const confirmPassword = control.get('ConfirmPassword')?.value;
    if (!password || !confirmPassword) {
      return null;
    }
    return password === confirmPassword ? null : { passwordMismatch: true };
  }
  seePassword() {
    this.showPassword = !this.showPassword;
  }
  seeConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  summit() {
    this.errorMessage = '';
    
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          alert("Đăng ký thành công!");
          this.router.navigate(['/login']);
        },
        error: (err) => {
          if (err.status === 400) {
            this.errorMessage = "Vui lòng nhập đầy đủ thông tin";
          } else if (err.status === 409) {
            this.errorMessage = "Email đã được sử dụng";
          } else {
            this.errorMessage = "Đã có lỗi xảy ra, vui lòng thử lại";
          }
          alert(this.errorMessage);
        }
      });   
    }
     else {
      this.registerForm.markAllAsTouched();
      if (this.registerForm.errors?.['passwordMismatch']) {
        alert("Mật khẩu và mật khẩu xác nhận không khớp nhau!");
      }
    }
  }


  
}