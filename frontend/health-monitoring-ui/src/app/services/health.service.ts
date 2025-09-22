import { Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { HttpClient } from '@angular/common/http';
import { HealthCheckDto } from '../models/health-check.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HealthService {
  private readonly baseApi = environment.baseApi;

  constructor(
    private readonly http: HttpClient
  ) { }

  getLatest(): Observable<HealthCheckDto[]> {
    const url = `${this.baseApi}/api/healthdata/latest`;
    return this.http.get<HealthCheckDto[]>(url);
  }

}