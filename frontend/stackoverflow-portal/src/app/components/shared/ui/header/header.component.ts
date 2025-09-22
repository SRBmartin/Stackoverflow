import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasicButtonComponent } from '../button/basic-button.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { SearchService } from '../../../../core/services/shared/search.service';

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
  constructor(private searchService: SearchService) {}

  isLoggedIn(): boolean {
    return true;
  }

  onLogin() { 
  }

  onSignup() {
  }

  onSearch(query: string) {
    this.searchService.setSearchTerm(query);
  }

  onProfile() { 
  }

  onAskQuestion() {  
  }
}

