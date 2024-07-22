import { Component, inject } from '@angular/core';
import { AlertComponent } from "../../components/alert/alert.component";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { AuthService } from "../../services/auth/auth.service";
import { AlertService } from "../../services/alert/alert.service";
import { FormInputComponent } from "../../components/shared/form-input/form-input.component";

@Component({
  selector: 'app-reset-password',
  standalone: true,
    imports: [AlertComponent, ReactiveFormsModule, FormInputComponent, RouterLink],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
    resetPasswordForm: FormGroup;
    token: string = '';
    username: string = '';

    private fb: FormBuilder = inject(FormBuilder);
    private route: ActivatedRoute = inject(ActivatedRoute);
    private authService: AuthService = inject(AuthService);
    private alertService: AlertService = inject(AlertService);

    constructor() {
        this.resetPasswordForm = this.fb.group({
            username: ['', [Validators.required]],
            token: ['', [Validators.required]],
            password: ['', [Validators.required]],
            confirmPassword: ['', [Validators.required]],
        });
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.token = params['token'];
            this.username = params['username'];

            // Set the token and username in the form controls
            this.resetPasswordForm.patchValue({
                token: this.token.replace(/\+/g, '%2B'),
                username: this.username,
            });
        });
    }

    onSubmit(): void {
        if (this.resetPasswordForm.valid) {
            this.authService.resetPassword(this.resetPasswordForm.value)
                .subscribe({
                    next: response => {
                        console.log(response.message);
                        this.alertService.showAlert('success', 'Password change successful.');
                        // Do whatever you want after successful password change.
                    },
                    error: error => {
                        console.error('Error:', error);
                        this.alertService.showAlert('danger', 'Password change failed.');
                    }
                })
        }
    }
}
