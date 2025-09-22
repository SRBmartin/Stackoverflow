import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { RouteNames } from "../../../components/shared/consts/routes";
import { RouterModule, RouterOutlet } from "@angular/router";
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
  standalone: true,
  imports: [
    RouterModule,
    RouterOutlet,
    CommonModule
  ]
})

export class ProfileComponent {
  constructor(public router: Router) {}

  isActive(url: string): boolean {
    return this.router.isActive(url, false); 
  }

  onLogout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}