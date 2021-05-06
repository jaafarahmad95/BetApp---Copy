import { Component, OnInit } from '@angular/core';
import { VisibleService } from 'src/app/_services/visible.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  constructor(private vs: VisibleService) { }

  ngOnInit(): void {
    this.vs.hide();
  }

  backHome (){
    this.vs.show();
  }

}
