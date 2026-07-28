export interface JwtPayload {
    sub: string;
    email: string;
    userId: string;
    firstName: string;
    lastName: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
}