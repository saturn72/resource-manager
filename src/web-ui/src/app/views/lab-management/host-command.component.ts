import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { HostService } from './../../services/host-service';

@Component({
  templateUrl: 'host-command.component.html'
})

export class HostCommandComponent {
  constructor(private route: ActivatedRoute, private hostService: HostService) { 
  }
  
  host = null;
  generalCommand = "";

  ngOnInit() {
    const hostId = this.route.snapshot.params["id"];
    this.host = this.hostService.getHostById(hostId);
  }

  sendGeneralCommand(event) {
    console.log("The general command is: " + this.generalCommand);
  }
}