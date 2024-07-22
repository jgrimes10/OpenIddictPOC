import { CanActivateFn, Router } from '@angular/router';
import { inject } from "@angular/core";
import { AuthService } from "../services/auth/auth.service";
import { AlertService } from '../services/alert/alert.service';

/**
 * Defines an authentication guard using Angular's `CanActivateFn` interface.
 * This guard is used to determine if a user can activate a route based on their authentication status.
 * If the user is logged in, they are allowed to proceed. Otherwise, they are redirected to the login page,
 * and a warning alert is shown.
 *
 * @param route The current route snapshot.
 * @param state The router state snapshot.
 * @returns A boolean indicating whether the user can activate the route.
 */
export const authGuard: CanActivateFn = (route, state) => {
    const authService: AuthService = inject(AuthService);
    const router: Router = inject(Router);
    const alertService: AlertService = inject(AlertService);

    if (authService.isLoggedIn()) {
        console.log('User is logged in, can go here.');
        return true;
    } else {
        console.log('You must be logged into navigate here.');
        alertService.showAlert('warning', 'You must be logged in.');
        router.navigate(['/login']);
        return false;
    }
};
