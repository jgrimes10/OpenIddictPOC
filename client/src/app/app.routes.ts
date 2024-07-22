import { Routes } from '@angular/router';
import { LoginComponent } from "./pages/login/login.component";
import { RegisterComponent } from "./pages/register/register.component";
import { HomeComponent } from "./pages/home/home.component";
import { authGuard } from "./guards/auth.guard";
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'forgotPassword', component: ForgotPasswordComponent },
    { path: 'home', component: HomeComponent, canActivate: [authGuard] },
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login' },
];
