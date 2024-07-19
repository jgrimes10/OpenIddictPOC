import { Component, inject } from '@angular/core';
import { AuthService } from "../../services/auth/auth.service";
import { Router } from "@angular/router";
import { NgIf } from "@angular/common";

@Component({
  selector: 'app-header',
  standalone: true,
    imports: [
        NgIf
    ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
    private authService: AuthService = inject(AuthService);
    private router: Router = inject(Router);

    get isLoggedIn(): boolean {
        return this.authService.isLoggedIn();
    }

    logout() {
        this.authService.logOut();
        this.router.navigate(['/']);
    }
}
