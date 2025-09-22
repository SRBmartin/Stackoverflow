import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-button',
  templateUrl: './basic-button.component.html',
  styleUrls: ['./basic-button.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ]
})
export class BasicButtonComponent {
  @Input() label: string = '';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() class: string = '';
  @Input() disabled: boolean = false;
  @Input() size: 'sm' | 'md' | 'lg' = 'sm';
  @Input() backgroundImage: string = '';
  @Input() routerLink?: string | any[];
  @Input() routerLinkActive?: string;   

  @Output() clickEvent: EventEmitter<void> = new EventEmitter<void>();

  constructor(private router: Router) {}

  onClick(): void {
    if (!this.disabled) {
      this.clickEvent.emit();
      if (this.routerLink) {
        this.router.navigate(Array.isArray(this.routerLink) ? this.routerLink : [this.routerLink]);
      }
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
