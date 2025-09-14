import { inject } from "@angular/core";
import { CanActivateFn, CanMatchFn, Router } from "@angular/router";
import { getToken, isJwtExpired, TOKEN_KEY } from "../utils/jwt.utils";

export const authGuard: CanMatchFn & CanActivateFn = () => {
    const router = inject(Router);
    const token = getToken();

    if (!token || isJwtExpired(token)) {
        if (token) localStorage.removeItem(TOKEN_KEY);
        return router.createUrlTree(['/login']);
    }

    return true;
}