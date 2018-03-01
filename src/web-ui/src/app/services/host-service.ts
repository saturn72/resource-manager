import { Injectable } from '@angular/core';

@Injectable()
export class HostService {
    getHostById(hostId) {
        return {
            id: hostId,
            friendlyName: "host fName",
            ipAddress: "some-ip address"
        };
    }
}
