import { Component, OnInit } from '@angular/core';
import { VisibleService } from '../_services/visible.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  collapseFooter:boolean = true;
  arrowUp:boolean = false;

  constructor(public vs: VisibleService) { }

  ngOnInit(): void {
  }
}
