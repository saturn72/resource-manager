import { Component } from '@angular/core';

@Component({
  templateUrl: 'hosts.component.html'
})

export class HostsComponent {
  allHosts = [{
    id: 1,
    ipAddress: "some-ip-address1",
    friendlyName: "some-friendly-name1",
    status: "active1"
  }, {
    id: 2,
    ipAddress: "some-ip-address2",
    friendlyName: "some-friendly-name2",
    status: "active2"
  }, {
    id: 3,
    ipAddress: "some-ip-address3",
    friendlyName: "some-friendly-name3",
    status: "active3"
  }, {
    id: 4,
    ipAddress: "some-ip-address4",
    friendlyName: "some-friendly-name4",
    status: "active4"
  }, {
    id: 5,
    ipAddress: "some-ip-address5",
    friendlyName: "some-friendly-name5",
    status: "active5"
  }];
}