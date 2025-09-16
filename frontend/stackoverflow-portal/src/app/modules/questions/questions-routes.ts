import { Routes } from "@angular/router";
import { authGuard } from "../../core/auth/guards/auth";

export const QUESTIONS_ROUTES: Routes = [
    {
        path: '',
        canMatch: [authGuard],
        loadComponent: () => import('./list-questions/list-questions.component').then(m => m.ListQuestionsComponent)
    }
];