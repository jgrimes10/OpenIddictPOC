import { Component, inject } from '@angular/core';
import { AlertComponent } from "../../components/alert/alert.component";
import { AlertService } from "../../services/alert/alert.service";

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [
        AlertComponent
    ],
    templateUrl: './home.component.html',
    styleUrl: './home.component.css'
})
export class HomeComponent {
    private alertService: AlertService = inject(AlertService);

    ngOnInit(): void {
        this.alertService.showAlert('success', 'You are logged in!');
    }
}
