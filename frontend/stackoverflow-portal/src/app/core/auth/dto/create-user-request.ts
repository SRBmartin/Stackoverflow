export interface CreateUserRequest {
    name: string;
    lastname: string;
    email: string;
    password: string;
    gender: string; // "M", "F"
    state: string;
    city: string;
    address: string;
}