export class UserRegistration{

    constructor(userName: string, email: string, password: string){
        this.userName = userName;
        this.email = email;
        this.password = password;
    }

    userName: string;
    email: string;
    password: string;
}