import { Component, inject } from '@angular/core';
import { RouterLink } from "@angular/router";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth/auth.service";
import { AlertService } from "../../services/alert/alert.service";
import { AlertComponent } from "../../components/alert/alert.component";
import { FormInputComponent } from '../../components/shared/form-input/form-input.component';

/**
 * The RegisterComponent is responsible for rendering the registration form and handling its submission.
 * It uses Angular's reactive forms module to create and manage the form and its validation.
 * On form submission, it interacts with the AuthService to register a new user and uses the AlertService
 * to provide feedback to the user.
 */
@Component({
  selector: 'app-register',
  standalone: true,
    imports: [
        RouterLink,
        ReactiveFormsModule,
        AlertComponent,
        FormInputComponent
    ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
    registerForm: FormGroup;

    private fb: FormBuilder = inject(FormBuilder);
    private authService: AuthService = inject(AuthService);
    private alertService: AlertService = inject(AlertService);

    constructor() {
        this.registerForm = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]],
            emailAddress: ['', [Validators.required, Validators.email]],
        });
    }

    /**
     * Handles the form submission. If the form is valid, it attempts to register the user
     * using the AuthService. On success, it may redirect or perform other actions. On failure,
     * it displays an error alert using the AlertService.
     */
    onSubmit() {
        if (this.registerForm.valid) {
            const registerData = this.registerForm.value;
            this.authService.register(registerData).subscribe({
                next: (response:any) => {
                    console.log('Registration successful', response);
                    // Handle success, maybe redirect to another page.
                },
                error: (error) => {
                    console.error('Registration error', error);
                    // Show error alert.
                    this.alertService.showAlert('danger', 'Registration failed.');
                }
            });
        } else {
            console.error("Registration form invalid");
        }
    }
}
