import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss']
})
export class ToastComponent {
  @Input() message: string = '';
  @Input() type: 'success' | 'error' | 'info' | 'warning' = 'info';
  @Input() visible: boolean = false;

  get bootstrapType(): string {
    switch (this.type) {
      case 'success': return 'success';   
      case 'error':   return 'danger';    
      case 'info':    return 'info';      
      case 'warning': return 'warning';   
      default: return 'secondary';
    }
  }
}
