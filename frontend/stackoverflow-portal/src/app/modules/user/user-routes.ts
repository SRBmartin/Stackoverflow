import { Routes } from "@angular/router";
import { notAuthGuard } from "../../core/auth/guards/not-auth";
import { authGuard } from "../../core/auth/guards/auth";

export const USER_ROUTES : Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'login' },
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
        path: 'profile',
        canMatch: [authGuard],
        loadChildren: () => import('./profile/profile.module').then(m => m.ProfileModule)
    }

];