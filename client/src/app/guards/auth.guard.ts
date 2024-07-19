import { CanActivateFn, Router } from '@angular/router';
import { inject } from "@angular/core";
import { AuthService } from "../services/auth/auth.service";

export const authGuard: CanActivateFn = (route, state) => {
    const authService: AuthService = inject(AuthService);
    const router: Router = inject(Router);

    if (authService.isLoggedIn()) {
        console.log('User is logged in, can go here.');
        return true;
    } else {
        console.log('You must be logged into navigate here.');
        router.navigate(['/login']);
        return false;
    }
};
