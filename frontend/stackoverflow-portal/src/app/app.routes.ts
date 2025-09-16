import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: 'questions',
        loadChildren: () => import('./modules/questions/questions-routes').then(m => m.QUESTIONS_ROUTES)
    },
    {
        path: '',
        loadChildren: () => import('./modules/user/user-routes').then(m => m.USER_ROUTES)
    },
    {
        path: '**',
        redirectTo: 'questions'
    }
];
