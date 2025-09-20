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
    formData.append('photo', file);

    return this.httpClient.post(`${this.usersUrl}/photo`, formData);
  }
}
