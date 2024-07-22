export interface LoginUser {
    Username: string
    Password: string
}

export interface RegisterUser {
    Username: string
    EmailAddress: string
    Password: string
}

export interface ResetPassword {
    Username: string;
    Token: string;
    Password: string;
    ConfirmPassword: string;
}
