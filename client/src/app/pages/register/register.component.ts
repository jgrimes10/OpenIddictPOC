import { Component, inject } from '@angular/core';
import { RouterLink } from "@angular/router";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth/auth.service";
import { AlertService } from "../../services/alert/alert.service";
import { AlertComponent } from "../../components/alert/alert.component";

@Component({
  selector: 'app-register',
  standalone: true,
    imports: [
        RouterLink,
        ReactiveFormsModule,
        AlertComponent
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
        });
    }

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
