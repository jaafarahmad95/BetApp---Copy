import { ThrowStmt } from '@angular/compiler';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class VisibleService {
  visible : boolean = true;
  //visibility
  marketSuspend: boolean = false;

  warnBetSlip: boolean = false;

  constructor() { }

  show() {
    this.visible = true;
  }

  hide () {
    this.visible = false;
  }

  toggle() {
    this.visible = !this.visible;
  }

}
