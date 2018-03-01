import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HostsComponent } from './hosts.component';
import { HostCommandComponent } from './host-command.component';

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Lab-Management'
    },
    children: [
      {
        path: 'hosts',
        component: HostsComponent,
        data: {
          title: 'Hosts'
        }
      }, {
        path: 'command/:id',
        component: HostCommandComponent,
        data: {
          title: 'Command Host'
        }
      }]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LabManagementRoutingModule { }
