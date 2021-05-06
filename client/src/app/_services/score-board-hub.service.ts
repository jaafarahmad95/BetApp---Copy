import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection,HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { Connections } from '../_models/Connections';
import { scoreboard } from '../_models/scoreboard';
import { User } from '../_models/user';
import { LiveService } from './live.service';
import { MatchesService } from './matches.service';
import { OddWorkService } from './odd-work.service';
import { ScoreboardService } from './scoreboard.service';
import { VisibleService } from './visible.service';

@Injectable({
  providedIn: 'root'
})
export class ScoreBoardHubService {
  baseUrl = environment.apiUrl;
  scoreHubUrl = environment.ScoreHubUrl;
  userr: User;
  private hubConnection: HubConnection;
  state = "Disconnected"
  constructor(private sc: ScoreboardService) { }

  creatHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder().withUrl(this.scoreHubUrl + '/', {
        accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build()

    this.hubConnection.start().then(() => {
      this.state = "Finished";
        console.log("connectionId from start"+this.hubConnection.connectionId);
        this.scoreboardListen();
        var x : Connections ={
          Username: "",
          ConnId : []
        }
        x.Username = user.username;
        x.ConnId.push(this.hubConnection.connectionId);
      this.sc.SbManageCon(x).subscribe();

    }).catch(error =>console.log(error.error));

  }
  updateSc(EventId: number){
    var groupName = 'event-'+EventId+'-update';
    //console.log('this is group '+ groupName);
    //console.log(groupName);
    this.hubConnection.invoke<any>('RemoveFromGroup', groupName).catch(error => {
      console.error(error);
    });
  }
  scoreboardListen(){
    this.hubConnection.on("ScoreBoardUpdate", (res : scoreboard)=> {
      //console.log('iam scoreboard listener this is my result');
      //console.log(res);
      this.sc.board = res;
    })
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
  stopHubConnection() {
    this.state = "Disconnected"
    this.hubConnection.stop().catch(error => console.log(error));
  }

}
