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

  uploadPhoto(file: File): Observable<any> {
  const formData = new FormData();
  formData.append('file', file);

  return this.httpClient.post(`${this.usersUrl}/photo`, formData);
}

  getProfile(): Observable<any> {
    return this.httpClient.get(`${this.usersUrl}/profile`);
  }

  getPhotoUrlById(userId: string): string {
    return `${environment.apiUrl}/users/${userId}/photo`;
  }
  getPhotoBlob(userId: string) {
    return this.httpClient.get(`${this.usersUrl}/${userId}/photo`, { responseType: 'blob' });
  }

  updateUser(data: { Name: string, Lastname: string, State: string, City: string, Address: string }): Observable<any> {
    return this.httpClient.put(`${this.usersUrl}/profile`, data);
  }
}
