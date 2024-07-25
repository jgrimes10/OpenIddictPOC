import { Component, inject } from '@angular/core';
import { NgIf } from "@angular/common";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth/auth.service";
import { FormInputComponent } from "../../components/shared/form-input/form-input.component";
import { AlertService } from "../../services/alert/alert.service";
import { AlertComponent } from "../../components/alert/alert.component";
import { QrCodeComponent } from "../../components/qr-code/qr-code.component";

@Component({
  selector: 'app-user-settings',
  standalone: true,
    imports: [
        NgIf,
        FormsModule,
        ReactiveFormsModule,
        FormInputComponent,
        AlertComponent,
        QrCodeComponent
    ],
  templateUrl: './user-settings.component.html',
  styleUrl: './user-settings.component.css'
})
export class UserSettingsComponent {
    enableMfaForm: FormGroup;

    // Dependency injection.
    private authService: AuthService = inject(AuthService);
    private fb: FormBuilder = inject(FormBuilder);
    private alertService: AlertService = inject(AlertService);

    enableAuthenticator: boolean = false;
    enableSms: boolean = false;
    isVerifying: boolean = false;
    qrCodeUri: string = '';

    constructor() {
        this.enableMfaForm = this.fb.group({
            username: ['', [Validators.required]],
            code: ['', !this.isVerifying ? null : Validators.required],
        });
    }

    ngOnInit(): void {
        // Get the user's current authenticator status and update the values here.
    }

    onSubmit() {
        if (this.enableMfaForm.valid) {
            const username = this.enableMfaForm.value['username'];
            const code = this.enableMfaForm.value['code'];
            // We're trying to enable first, which will return a secret/code to verify.
            if (!this.isVerifying) {
                this.authService.enableMfaAuthenticator(username).subscribe({
                    next: response => {
                        console.log(response);
                        this.isVerifying = true;
                        this.qrCodeUri = response.qrCodeUri
                    },
                    error: err => {
                        this.alertService.showAlert("danger", "Failed to enable Mfa.");
                        console.error(err);
                    }
                })
            } else {
                // We are now trying to verify that we have the authenticator before enabling it.
                this.authService.confirmAuthenticatorToEnable(username, code).subscribe({
                    next: response => {
                        console.log(response);
                        // Handle success
                        this.alertService.showAlert('success', 'Authenticator enabled successfully.');
                    },
                    error: err => {
                        this.alertService.showAlert('danger', 'Failed to confirm Mfa.');
                        console.error(err);
                    }
                })
            }
        }
    }
}
