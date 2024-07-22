import { Component, inject } from '@angular/core';
import { AlertComponent } from '../../components/alert/alert.component';
import { FormInputComponent } from '../../components/shared/form-input/form-input.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { AlertService } from '../../services/alert/alert.service';

@Component({
    selector: 'app-forgot-password',
    standalone: true,
    imports: [
        AlertComponent,
        FormInputComponent,
        ReactiveFormsModule,
        RouterLink
    ],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
    forgotPasswordForm: FormGroup;

    // Dependency injection
    private fb: FormBuilder = inject(FormBuilder);
    private router: Router = inject(Router);
    private authService: AuthService = inject(AuthService);
    private alertService: AlertService = inject(AlertService);

    constructor() {
        this.forgotPasswordForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]]
        });
    }

    onSubmit(): void {
        if (this.forgotPasswordForm.valid) {
            this.authService.forgotPassword(this.forgotPasswordForm.value.email)
                .subscribe({
                    next: (response) => {
                        // Handle successful response
                        this.alertService.showAlert('success', 'Forgot password email sent.');
                        console.log('Sending forgot password email');
                    },
                    error: (error) => {
                        // Handle error response
                        this.alertService.showAlert('danger', 'Something went wrong sending the forgot password email.');
                        console.error('Error sending forgot password email');
                    },
                });
        }
    }
}
