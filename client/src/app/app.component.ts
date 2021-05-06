import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './_models/user';
import { authService } from './_services/auth.Service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'The Bet App';


  constructor (private authService: authService,
    private presence: PresenceService) {}

  ngOnInit(): void {
    this.setCurrentuser();
  }

  setCurrentuser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user){
      this.authService.setCurrentUser(user);
      //this.router.navigateByUrl('/');
      //when create it we get access to the jwt token
      //this.presence.creatHubConnection(user);
    }

  }



}
