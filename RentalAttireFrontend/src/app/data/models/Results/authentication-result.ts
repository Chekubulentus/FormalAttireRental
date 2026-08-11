import { UserDTO } from "../DTOs/Users/user-dto";


export class AuthenticationResult {
    accessToken : string = '';
    refreshToken : string = '';
    expiresAt : Date = new Date();
    id: number = 0;
    email: string = '';
    isProfileComplete: boolean = false;
}