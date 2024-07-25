import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable, tap } from "rxjs";

import { RegisterUser, LoginUser, ResetPassword } from "../../models/User";

/**
 * AuthService provides authentication-related functionalities, including user registration,
 * login, and session management using JWT tokens. It interacts with a backend server to
 * register new users, log in existing users, and manage session tokens.
 */
@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private http: HttpClient = inject(HttpClient);

    private registerUrl = 'https://localhost:5001/register';
    private loginUrl = 'https://localhost:5001/connect/token';
    private forgotPasswordUrl = 'https://localhost:5001/forgot-password';
    private resetPasswordUrl = 'https://localhost:5001/reset-password';
    private enableAuthenticatorUrl = 'https://localhost:5001/enable-authenticator';
    private confirmAuthenticatorToEnableUrl = 'https://localhost:5001/confirm-authenticator';
    private disableAuthenticatorUrl = 'https://localhost:5001/disable-authenticator';

    constructor() { }

    /**
     * Registers a new user with the provided user data.
     * @param data The user data for registration.
     * @returns An Observable of the HTTP response from the registration endpoint.
     */
    register(data: RegisterUser): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(data);
        return this.http.post(this.registerUrl, body, { headers });
    }

    /**
     * Logs in a user with the provided credentials.
     * @param data The login credentials.
     * @returns An Observable of the HTTP response from the login endpoint.
     */
    login(data: LoginUser): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(data);
        console.log(body);
        return this.http.post(this.loginUrl, body, { headers }).pipe(
            tap({
                next: (response: any) => {
                    this.handleAuthSuccess(response);
                }
            })
        );
    }

    enableMfaAuthenticator(username: string): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded({ username });
        console.log(`Sending request to ${this.enableAuthenticatorUrl} with body ${body}`);
        return this.http.post(this.enableAuthenticatorUrl, body, { headers });
    }

    confirmAuthenticatorToEnable(username: string, code: string): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded({ username, code });
        console.log(`Sending request to ${this.enableAuthenticatorUrl} with body ${body}`);
        return this.http.post(this.confirmAuthenticatorToEnableUrl, body, { headers });
    }

    disableMfaAuthenticator(username: string): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(username);
        return this.http.post(this.disableAuthenticatorUrl, body, { headers });
    }

    /**
     * Converts an object into a URL-encoded string format.
     * @param obj The object to be URL-encoded.
     * @returns A URL-encoded string representation of the object.
     */
    private toUrlEncoded(obj: any): string {
        return Object.keys(obj).map(key => encodeURIComponent(key) + "=" + encodeURIComponent(obj[key])).join('&');
    }

    /**
     * Handles successful authentication by storing session tokens in localStorage.
     * @param response The HTTP response containing authentication tokens.
     */
    private handleAuthSuccess(response: any): void {
        localStorage.setItem('access_token', response.access_token);
        localStorage.setItem('refresh_token', response.refresh_token);
        localStorage.setItem('expires_in', response.expires_in);
        localStorage.setItem('scope', response.scope);
        localStorage.setItem('token_type', response.token_type);
    }

    /**
     * Retrieves the access token from localStorage.
     * @returns The access token if it exists, otherwise null.
     */
    getToken(): string | null {
        return localStorage.getItem('access_token');
    }

    /**
     * Checks if the user is currently logged in based on the presence of an access token.
     * @returns true if the user is logged in, otherwise false.
     */
    isLoggedIn(): boolean {
        return this.getToken() !== null;
    }

    /**
     * Logs out the current user by removing session tokens from localStorage.
     */
    logOut(): void {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('expires_in');
        localStorage.removeItem('scope');
        localStorage.removeItem('token_type');
    }

    forgotPassword(username: string): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded({ username });
        return this.http.post(this.forgotPasswordUrl, body, { headers });
    }

    resetPassword(data: any): Observable<any> {
        const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
        const body = this.toUrlEncoded(data);
        return this.http.post(this.resetPasswordUrl, body, { headers });
    }
}
