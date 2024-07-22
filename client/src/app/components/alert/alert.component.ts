import { Component, inject, Input } from '@angular/core';
import { NgClass, NgIf } from "@angular/common";
import { AlertService } from "../../services/alert/alert.service";
import { SuccessIconComponent } from "../icons/success-icon/success-icon.component";
import { InfoIconComponent } from "../icons/info-icon/info-icon.component";
import { WarningIconComponent } from "../icons/warning-icon/warning-icon.component";
import { DangerIconComponent } from "../icons/danger-icon/danger-icon.component";

/**
 * AlertComponent is responsible for displaying alert messages of various types (success, info, warning, danger).
 * It subscribes to the AlertService's alertState to receive and display alert messages.
 * This component also handles the dynamic application of CSS classes based on the alert type to style the alert message accordingly.
 */
@Component({
    selector: 'app-alert',
    standalone: true,
    imports: [
        NgClass,
        NgIf,
        SuccessIconComponent,
        InfoIconComponent,
        WarningIconComponent,
        DangerIconComponent
    ],
    templateUrl: './alert.component.html',
    styleUrl: './alert.component.css'
})
export class AlertComponent {
    alert: { type: 'success' | 'info' | 'warning' | 'danger', message: string } | null = null;

    private alertService: AlertService = inject(AlertService);

    /**
     * Subscribes to the alertState observable of AlertService on component initialization to receive alert messages.
     */
    ngOnInit(): void {
        this.alertService.alertState.subscribe({
            next: data => {
                this.alert = data;
            }
        });
    }

    /**
     * Clears the current alert message by calling the clearAlert method of AlertService.
     */
    closeAlert(): void {
        this.alertService.clearAlert();
    }

    /**
     * Computes and returns the CSS classes for the alert based on its type.
     * This enables dynamic styling of the alert message.
     * @returns An object where each key is a CSS class and its value is a boolean indicating whether the class should be applied.
     */
    get alertClasses() {
        return {
            'bg-green-100 border-green-400 text-green-700': this.alert?.type === 'success',
            'bg-blue-100 border-blue-400 text-blue-700': this.alert?.type === 'info',
            'bg-yellow-100 border-yellow-400 text-yellow-700': this.alert?.type === 'warning',
            'bg-red-100 border-red-400 text-red-700': this.alert?.type === 'danger',
        }
    };
}
