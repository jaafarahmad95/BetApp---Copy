import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { VisibleService } from 'src/app/_services/visible.service';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  error: any;

  constructor(private router: Router, private vs: VisibleService) {

    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error;
  }

  ngOnInit(): void {
    this.vs.hide();
  }

}
