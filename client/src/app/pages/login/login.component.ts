import { Component, inject } from '@angular/core';
import { Router, RouterLink } from "@angular/router";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth/auth.service";
import { AlertComponent } from "../../components/alert/alert.component";
import { AlertService } from "../../services/alert/alert.service";

@Component({
  selector: 'app-login',
  standalone: true,
    imports: [
        RouterLink,
        ReactiveFormsModule,
        AlertComponent
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

    constructor() {
        this.loginForm = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]],
        });
    }

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
