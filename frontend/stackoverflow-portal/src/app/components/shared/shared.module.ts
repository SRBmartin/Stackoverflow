import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';  
import { HeaderComponent } from './ui/header/header.component';
import { FooterComponent } from './ui/footer/footer.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SearchInputComponent } from './ui/search-input/search-input.component';
import { BasicButtonComponent } from './ui/button/basic-button.component';


@NgModule({
  declarations: [
    BasicButtonComponent,
    HeaderComponent,
    FooterComponent,
    SearchInputComponent,
  ],
  exports: [
    BasicButtonComponent,
    HeaderComponent,
    FooterComponent,
    SearchInputComponent,
  ],
  imports: [CommonModule]
})
export class SharedModule {}

