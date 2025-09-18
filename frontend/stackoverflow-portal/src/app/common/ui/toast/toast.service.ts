import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface ToastData {
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
}

@Injectable({
  providedIn: 'root'
})
export class ToastServer {
  private toastSubject = new BehaviorSubject<ToastData | null>(null);
  toastState$ = this.toastSubject.asObservable();

  showToast(message: string, type: 'success' | 'error' | 'info' | 'warning' = 'info') {
    this.toastSubject.next({ message, type });

    setTimeout(() => this.clearToast(), 3000);
  }

  clearToast() {
    this.toastSubject.next(null);
  }
}
