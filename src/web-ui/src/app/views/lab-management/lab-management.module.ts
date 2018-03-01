// Angular
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';

import { HostsComponent } from './hosts.component';
import { HostCommandComponent } from './host-command.component';

// Components Routing
import { LabManagementRoutingModule } from './lab-management-routing.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    LabManagementRoutingModule
  ],
  declarations: [
    HostsComponent,
    HostCommandComponent
  ]
})
export class LabManagementModule { }