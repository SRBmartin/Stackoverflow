import { inject } from "@angular/core";
import { CanMatchFn, Router } from "@angular/router";
import { TOKEN_KEY } from "../utils/jwt.utils";

export const notAuthGuard: CanMatchFn = () => {
    const isAuthed = !!localStorage.getItem(TOKEN_KEY);

    return isAuthed ? inject(Router).createUrlTree(['/questions']) : true;
}