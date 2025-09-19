import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasicButtonComponent } from '../button/basic-button.component';
import { SearchInputComponent } from '../search-input/search-input.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    BasicButtonComponent, 
    SearchInputComponent
  ]
})
export class HeaderComponent {
  isLoggedIn(): boolean {
    return true;
  }

  onLogin() { 
  }

  onSignup() {
  }

  onSearch(query: string) {
  }

  onProfile() { 
  }

  onAskQuestion() {  
  }
}

