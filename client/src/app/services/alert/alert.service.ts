import { Injectable } from '@angular/core';
import { BehaviorSubject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AlertService {
    private alertSubject = new BehaviorSubject<{ type: 'success' | 'info' | 'warning' | 'danger', message: string } | null>(null);
    alertState = this.alertSubject.asObservable();

    constructor() { }

    showAlert(type: 'success' | 'info' | 'warning' | 'danger', message: string): void {
        this.alertSubject.next({ type, message });
    }

    clearAlert(): void {
        this.alertSubject.next(null);
    }
}
