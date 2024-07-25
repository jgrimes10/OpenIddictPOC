export interface LoginUser {
    Username: string
    Password: string
    mfaCode: string | undefined
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
