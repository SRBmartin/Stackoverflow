export interface AuthResponseDto {
    AccessToken: string;
    TokenType: string;
    ExpiresAt: string; //ISO from DateTimeOffset
    UserId: string;
    Email: string;
    Name: string;
    Lastname: string;
}