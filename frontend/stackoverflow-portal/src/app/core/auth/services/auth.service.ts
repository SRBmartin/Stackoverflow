import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../env/environment';
import { LoginUserRequest } from '../dto/login-user-request';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { AuthResponseDto } from '../dto/auth-response-dto';
import { TOKEN_KEY } from '../utils/jwt.utils';
import { CreateUserRequest } from '../dto/create-user-request';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly httpClient: HttpClient = inject(HttpClient);

  private readonly usersUrl = `${environment.apiUrl}/users`;

  login(request: LoginUserRequest): Observable<AuthResponseDto> {
    return this.httpClient.post<AuthResponseDto>(`${this.usersUrl}/login`, request)
      .pipe(
        map(response => {
          if (!response?.AccessToken || typeof response.AccessToken !== 'string' || !response.AccessToken.trim()) {
            throw new Error('Missing AccessToken in response');
          }
          localStorage.setItem(TOKEN_KEY, response.AccessToken);
          return response;
        }),
        catchError(err => {
          localStorage.removeItem(TOKEN_KEY);
          return throwError(() => err);
        })
      );
  }

  register(request: CreateUserRequest): Observable<AuthResponseDto> {
    return this.httpClient.post<AuthResponseDto>(this.usersUrl, request)
      .pipe(
        map(response => {
          if (!response?.AccessToken || typeof response.AccessToken !== 'string' || !response.AccessToken.trim()) {
            throw new Error('Missing AccessToken in response');
          }
          localStorage.setItem(TOKEN_KEY, response.AccessToken);
          return response;
        }),
        catchError(err => {
          localStorage.removeItem(TOKEN_KEY);
          return throwError(() => err);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

}
