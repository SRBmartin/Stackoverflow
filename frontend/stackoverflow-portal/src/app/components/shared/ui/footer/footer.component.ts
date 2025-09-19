import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { BasicButtonComponent } from '../button/basic-button.component';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [
    CommonModule,
    BasicButtonComponent
  ],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {

}
