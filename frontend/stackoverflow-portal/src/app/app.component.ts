import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './components/shared/ui/header/header.component';
import { FooterComponent } from './components/shared/ui/footer/footer.component';
import { LoaderComponent } from './common/ui/loader/loader.component';
import { ToastComponent } from './common/ui/toast/toast.component';
import { ToastServer, ToastData } from './common/ui/toast/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    LoaderComponent,
    ToastComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  toast$ = this.toastServer.toastState$;

  constructor(private toastServer: ToastServer) {
    
    this.toast$.subscribe((t: ToastData | null) => {
      
    });
  }
}
