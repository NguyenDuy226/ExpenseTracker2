import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

export interface LoginRequest {
  Email: string;
  Password: string;
}

export interface RegisterRequest{
  Name: string;
  Email: string;
  Password: string;
  ConfirmPassword: string;
}

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  constructor (
    private http : HttpClient,
    private router : Router
  ){}
  login(request: LoginRequest) {
    return this.http.post<any>('http://localhost:5076/api/auth/login', request);    
  }
  register (request: RegisterRequest){
    return this.http.post<any>('http://localhost:5076/api/auth/register', request);    
  }
  getToken() {
    return localStorage.getItem('token');
  }
  isLoggedIn() {
    var token = this.getToken();
    if(token == null) return false;
    else return true;
  }
  logout (){
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
 
}


