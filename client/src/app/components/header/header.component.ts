import { Component, inject } from '@angular/core';
import { AuthService } from "../../services/auth/auth.service";
import { Router, RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { AlertService } from "../../services/alert/alert.service";

/**
 * HeaderComponent is responsible for displaying the application's header.
 * It includes navigation links, and dynamically shows the logout option based on the user's authentication status.
 * It utilizes AuthService to check if a user is logged in and to perform logout operations.
 * On logout, it uses AlertService to notify the user of successful logout and navigates back to the home route
 * (which will push back to the login page).
 */
@Component({
  selector: 'app-header',
  standalone: true,
    imports: [
        NgIf,
        RouterLink
    ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
    private authService: AuthService = inject(AuthService);
    private router: Router = inject(Router);
    private alertService: AlertService = inject(AlertService);

    /**
     * Determines if the user is currently logged in by checking the authentication status through AuthService.
     * @returns boolean indicating the user's authentication status.
     */
    get isLoggedIn(): boolean {
        return this.authService.isLoggedIn();
    }

    /**
     * Performs logout operation by calling AuthService's logOut method.
     * It then displays a logout success message using AlertService and navigates the user back to the home route.
     */
    logout() {
        this.authService.logOut();
        this.alertService.showAlert('info', 'Successfully logged out.');
        this.router.navigate(['/']);
    }
}
