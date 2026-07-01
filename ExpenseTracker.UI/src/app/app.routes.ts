import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: 'login',
        loadComponent: ()=> import('./Components/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'register',
        loadComponent: ()=>import('./Components/register/register.component').then(m => m.RegisterComponent)
    },
    {
        path: 'dashboard',
        canActivate: [authGuard],
        loadComponent: ()=>import('./Components/dashboard/dashboard.component').then(m =>m.DashboardComponent)
    },
    {
        path: 'transactions',
        canActivate: [authGuard],
        loadComponent: ()=>import('./Components/transactions/transactions.component').then(m => m.TransactionsComponent)
    },
    {
        path: 'reports',
        canActivate: [authGuard],
        loadComponent: ()=>import('./Components/reports/reports.component').then(m => m.ReportsComponent)
    },
    {
        path: '**',
        redirectTo: 'login'    
     },
];
