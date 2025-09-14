import { Routes } from "@angular/router";
import { notAuthGuard } from "../../core/auth/guards/not-auth";

export const USER_ROUTES : Routes = [
    {
        path: 'login',
        canMatch: [notAuthGuard],
        loadComponent: () => import('./login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'register',
        canMatch: [notAuthGuard],
        loadComponent: () => import('./register/register.component').then(m => m.RegisterComponent)
    },
    {
        path: '**',
        redirectTo: 'login'
    }
];