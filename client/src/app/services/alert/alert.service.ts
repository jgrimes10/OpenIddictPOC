import { Injectable } from '@angular/core';
import { BehaviorSubject } from "rxjs";

/**
 * Provides a service for managing alert messages across the application.
 * Utilizes a BehaviorSubject to maintain the current alert state, allowing components to subscribe
 * and react to changes in alert status.
 */
@Injectable({
    providedIn: 'root'
})
export class AlertService {
    private alertSubject = new BehaviorSubject<{ type: 'success' | 'info' | 'warning' | 'danger', message: string } | null>(null);
    alertState = this.alertSubject.asObservable();

    constructor() { }

    /**
     * Triggers an alert of a specified type with a message.
     * This will update the alert state, which can be subscribed to by components.
     *
     * @param type The type of the alert ('success', 'info', 'warning', 'danger').
     * @param message The message to be displayed in the alert.
     */
    showAlert(type: 'success' | 'info' | 'warning' | 'danger', message: string): void {
        this.alertSubject.next({ type, message });
    }

    /**
     * Clears the current alert state, effectively removing any displayed alerts.
     */
    clearAlert(): void {
        this.alertSubject.next(null);
    }
}
