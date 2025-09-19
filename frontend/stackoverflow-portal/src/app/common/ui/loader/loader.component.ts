import { Component } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { LoaderService } from './loader.service';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule, AsyncPipe],
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent {
  constructor(public loaderService: LoaderService) {}
}
