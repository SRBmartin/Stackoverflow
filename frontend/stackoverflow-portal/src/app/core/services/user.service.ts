import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly httpClient: HttpClient = inject(HttpClient);
  private readonly usersUrl = `${environment.apiUrl}/users`;

  register(user: any): Observable<any> {
    return this.httpClient.post(`${this.usersUrl}`, user);
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.httpClient.post(`${this.usersUrl}/login`, credentials);
  }

  uploadPhoto(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    const token = localStorage.getItem('token') || '';
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    return this.httpClient.post(`${this.usersUrl}/photo`, formData, { headers });
  }
}
