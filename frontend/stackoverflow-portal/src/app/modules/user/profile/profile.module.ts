import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GeneralInformationComponent } from './general-information/general-information.component';

import { ProfileRoutingModule } from './profile-routing.module';


@NgModule({
  imports: [
    CommonModule,
    ProfileRoutingModule
  ]
})
export class ProfileModule { }
