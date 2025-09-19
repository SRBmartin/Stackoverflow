import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasicButtonComponent } from '../button/basic-button.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { getToken, isJwtExpired } from '../../../../core/auth/utils/jwt.utils'
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    BasicButtonComponent, 
    SearchInputComponent,
    RouterModule
  ]
})
export class HeaderComponent {
  isLoggedIn(): boolean {
    const token = getToken();
    if (!token) return false;
    return !isJwtExpired(token);
  }

  onSearch(query: string) {
  }
}

