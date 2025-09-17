import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common'; // OBAVEZNO za ngIf

@Component({
  selector: 'app-search-input',
  templateUrl: './search-input.component.html',
  styleUrls: ['./search-input.component.scss'],
  standalone: true,          // <--- ovo je ključno
  imports: [CommonModule]     // <--- za *ngIf, *ngFor, itd.
})
export class SearchInputComponent {
  @Input() placeholder: string = 'Search...';
  @Output() searchChanged: EventEmitter<string> = new EventEmitter<string>();
  searchQuery: string = '';

  onInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery = input.value;
    this.searchChanged.emit(this.searchQuery);
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.searchChanged.emit(this.searchQuery);
  }
}
