import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, LoginRequest } from '../../core/services/auth.service';

@Component({
  selector: 'app-login.component',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent{
  errorMessage: string = '';
  showPassword = false;
  loginForm!: FormGroup;

  private formBuilder = inject(FormBuilder); 
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit (){
    this.loginForm = this.formBuilder.group({
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6)]],
    })
  }
  seePassword() {
    this.showPassword = !this.showPassword;
  }

   summit() {
      this.errorMessage = '';
      
      if (this.loginForm.valid) {
        this.authService.login(this.loginForm.value).subscribe({
          next: (reponse) => {
            localStorage.setItem('token', reponse.token)
            this.router.navigate(['/dashboard']);
          },
          error: (err) => {    
            console.error(err);
            if (err.status == 400) {
              this.errorMessage = "Vui lòng nhập đầy đủ thông tin";
            } 
            else if (err.status == 404){
              this.errorMessage = "Email không tồn tại";
            } 
            else if(err.status == 401){
              this.errorMessage = "Mật khẩu không chính xác"
            }
            else {
              this.errorMessage = "Đã có lỗi xảy ra, vui lòng thử lại";
            }
            alert(this.errorMessage);
          }
        });   
      }
      else {
        this.loginForm.markAllAsTouched();
      }
    }

}
