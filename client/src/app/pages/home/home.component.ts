import { Component, inject } from '@angular/core';
import { AlertComponent } from "../../components/alert/alert.component";
import { AlertService } from "../../services/alert/alert.service";

/**
 * HomeComponent is the main page component that users see after they log in.
 * It demonstrates the use of the AlertService to show a success message upon initialization
 * as well as the route guard that prevents unauthenticated users from accessing this page.
 */
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

    /**
     * ngOnInit is a lifecycle hook that is called after Angular has initialized all data-bound properties.
     * Here, it's used to show a success alert message indicating that the user is logged in.
     */
    ngOnInit(): void {
        this.alertService.showAlert('success', 'You are logged in!');
    }
}
