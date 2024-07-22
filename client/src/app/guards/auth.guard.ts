import { CanActivateFn, Router } from '@angular/router';
import { inject } from "@angular/core";
import { AuthService } from "../services/auth/auth.service";
import { AlertService } from '../services/alert/alert.service';

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
