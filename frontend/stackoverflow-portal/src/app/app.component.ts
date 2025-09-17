import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./common/ui/header/header.component";
import { FooterComponent } from "./common/ui/footer/footer.component";
import { CommonModule } from '@angular/common';
import { LoaderComponent } from './common/ui/loader/loader.component';
import { ToastComponent } from './common/ui/toast/toast.component';
import { ToastServer } from './common/ui/toast/toast.service';
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
  styleUrl: './app.component.scss'
})
export class AppComponent {
  toast$ = this.toastServer.toastState$;

  constructor(private toastServer: ToastServer) {}
}
