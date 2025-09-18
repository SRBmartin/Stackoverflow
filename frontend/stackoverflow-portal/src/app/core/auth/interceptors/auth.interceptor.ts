import { HttpInterceptorFn } from "@angular/common/http";
import { getToken, isJwtExpired } from "../utils/jwt.utils";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const token = getToken();

    if (token && !isJwtExpired(token)) {
        req = req.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
        });
    }

    return next(req);
}