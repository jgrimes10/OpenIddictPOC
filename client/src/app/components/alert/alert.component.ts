import { Component, inject, Input } from '@angular/core';
import { NgClass, NgIf } from "@angular/common";
import { AlertService } from "../../services/alert/alert.service";

@Component({
    selector: 'app-alert',
    standalone: true,
    imports: [
        NgClass,
        NgIf
    ],
    templateUrl: './alert.component.html',
    styleUrl: './alert.component.css'
})
export class AlertComponent {
    alert: { type: 'success' | 'info' | 'warning' | 'danger', message: string } | null = null;

    private alertService: AlertService = inject(AlertService);

    ngOnInit(): void {
        this.alertService.alertState.subscribe({
            next: data => {
                this.alert = data;
            }
        });
    }

    closeAlert(): void {
        this.alertService.clearAlert();
    }

    get alertClasses() {
        return {
            'bg-green-100 border-green-400 text-green-700': this.alert?.type === 'success',
            'bg-blue-100 border-blue-400 text-blue-700': this.alert?.type === 'info',
            'bg-yellow-100 border-yellow-400 text-yellow-700': this.alert?.type === 'warning',
            'bg-red-100 border-red-400 text-red-700': this.alert?.type === 'danger',
        }
    };
}
