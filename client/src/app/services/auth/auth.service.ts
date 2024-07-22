import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, tap } from "rxjs";

import { RegisterUser, LoginUser } from "../../models/User";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private http: HttpClient = inject(HttpClient);

    private registerUrl = 'https://localhost:5001/register';
    private loginUrl = 'https://localhost:5001/connect/token';
    private forgotPasswordUrl = 'https://localhost:5001/forgot-password';

    constructor() { }

    register(data: RegisterUser): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(data);
        return this.http.post(this.registerUrl, body, { headers });
    }

    login(data: LoginUser): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(data);
        return this.http.post(this.loginUrl, body, { headers }).pipe(
            tap({
                next: (response: any) => {
                    this.handleAuthSuccess(response);
                }
            })
        );
    }

    private toUrlEncoded(obj: any): string {
        return Object.keys(obj).map(key => encodeURIComponent(key) + "=" + encodeURIComponent(obj[key])).join('&');
    }

    private handleAuthSuccess(response: any): void {
        localStorage.setItem('access_token', response.access_token);
        localStorage.setItem('refresh_token', response.refresh_token);
        localStorage.setItem('expires_in', response.expires_in);
        localStorage.setItem('scope', response.scope);
        localStorage.setItem('token_type', response.token_type);
    }

    getToken(): string | null {
        return localStorage.getItem('access_token');
    }

    isLoggedIn(): boolean {
        return this.getToken() !== null;
    }

    logOut(): void {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('expires_in');
        localStorage.removeItem('scope');
        localStorage.removeItem('token_type');
    }

    forgotPassword(emailAddress: string): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded({ emailAddress });
        return this.http.post(this.forgotPasswordUrl, body, { headers });
    }
}
