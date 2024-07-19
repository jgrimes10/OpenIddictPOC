import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./components/header/header.component";
import { AlertComponent } from "./components/alert/alert.component";
import { SuccessIconComponent } from "./components/icons/success-icon/success-icon.component";
import { InfoIconComponent } from "./components/icons/info-icon/info-icon.component";
import { WarningIconComponent } from "./components/icons/warning-icon/warning-icon.component";
import { DangerIconComponent } from "./components/icons/danger-icon/danger-icon.component";

@Component({
  selector: 'app-root',
  standalone: true,
    imports: [RouterOutlet, HeaderComponent, AlertComponent, SuccessIconComponent, InfoIconComponent, WarningIconComponent, DangerIconComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';
}
