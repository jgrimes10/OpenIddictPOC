import { Component, inject } from '@angular/core';
import { AuthService } from "../../services/auth/auth.service";
import { Router, RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { AlertService } from "../../services/alert/alert.service";

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

    get isLoggedIn(): boolean {
        return this.authService.isLoggedIn();
    }

    logout() {
        this.authService.logOut();
        this.alertService.showAlert('info', 'Successfully logged out.');
        this.router.navigate(['/']);
    }
}
