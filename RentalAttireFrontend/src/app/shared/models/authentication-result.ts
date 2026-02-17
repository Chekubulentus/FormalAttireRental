import { UserDTO } from "../../data/models/user-dto";


export class AuthenticationResult {
    accessToken : string = '';
    refreshToken : string = '';
    expiresAt : Date = new Date();
    user : UserDTO = new UserDTO();
}