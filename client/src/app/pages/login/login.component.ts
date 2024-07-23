import { Component, inject } from '@angular/core';
import { Router, RouterLink } from "@angular/router";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth/auth.service";
import { AlertComponent } from "../../components/alert/alert.component";
import { AlertService } from "../../services/alert/alert.service";
import { FormInputComponent } from '../../components/shared/form-input/form-input.component';

/**
 * LoginComponent is responsible for handling user login functionality.
 * It creates and manages a login form, validates user input, and uses AuthService
 * to authenticate users. Upon successful login, it navigates to the home page.
 * In case of login failure, it displays an error message using AlertService.
 */
@Component({
  selector: 'app-login',
  standalone: true,
    imports: [
        RouterLink,
        ReactiveFormsModule,
        AlertComponent,
        FormInputComponent
    ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
    loginForm: FormGroup;

    // Dependency injection.
    private fb: FormBuilder = inject(FormBuilder);
    private router: Router = inject(Router);
    private authService: AuthService = inject(AuthService);
    private alertService: AlertService = inject(AlertService);

    qrCodeUri = 'otpauth://totp/JDOE@TEST.COM?secret=EKPCLLN766VPQKDAFFBPPIOBQH6YGY5A&issuer=IdentityAPI&digits=6';

    constructor() {
        this.loginForm = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]],
        });
    }

    /**
     * Handles the form submission. If the form is valid, it attempts to log in the user
     * using the AuthService. On success, it navigates to the home page. On failure,
     * it displays an error alert using the AlertService.
     */
    onSubmit() {
        if (this.loginForm.valid) {
            const loginData = this.loginForm.value;
            loginData.grant_type = 'password';
            loginData.client_id = 'default-client';
            loginData.client_secret = 'this-is-a-long-secret-but-should-be-longer';

            this.authService.login(loginData).subscribe({
                next: (response: any) => {
                    console.log('Login successful', response);
                    // Handle success, maybe redirect to another page.
                    this.router.navigate(['/home']);
                },
                error: (error) => {
                    console.error('Login error', error);
                    // Show error alert.
                    this.alertService.showAlert('danger', 'Login failed.');
                }
            });
        }
    }
}
