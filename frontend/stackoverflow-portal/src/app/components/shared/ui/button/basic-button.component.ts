import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  templateUrl: './basic-button.component.html',
  styleUrls: ['./basic-button.component.scss'],
  standalone: true,
  imports: [CommonModule] // neophodno za ngStyle
})
export class BasicButtonComponent {
  @Input() label: string = '';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() class: string = '';
  @Input() disabled: boolean = false;
  @Input() size: 'sm' | 'md' | 'lg' = 'sm';
  @Input() backgroundImage: string = '';

  @Output() clickEvent: EventEmitter<void> = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled) {
      this.clickEvent.emit();
    }
  }

  get getClass(): string {
    let baseClass = 'btn';
    if (this.size === 'sm') baseClass += ' btn-sm';
    if (this.size === 'md') baseClass += ' btn-md';
    if (this.size === 'lg') baseClass += ' btn-lg';

    return `${baseClass} ${this.class}`;
  }

  getStyles(): { [key: string]: string } {
    return this.backgroundImage
      ? { 'background-image': `url(${this.backgroundImage})`, 'background-size': 'cover' }
      : {};
  }
}
