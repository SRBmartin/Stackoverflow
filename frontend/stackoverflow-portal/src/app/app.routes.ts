import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadChildren: () => import('./modules/user/user-routes').then(m => m.USER_ROUTES)
    },
    {
        path: '**',
        redirectTo: 'login'
    }
];
