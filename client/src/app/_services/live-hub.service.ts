import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection,HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Connections } from '../_models/Connections';
import { live } from '../_models/live';
import { Odds } from '../_models/Odds';
import { placedBet } from '../_models/placedBet';
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
export class LiveHubService {
  baseUrl = environment.apiUrl;
  livHubUrl = environment.livHubUrl;
  userr: User;
  private hubConnection: HubConnection;
  private oddsThreadSourceLive = new BehaviorSubject<Odds[]>([]);
  oddsThreadLive$ = this.oddsThreadSourceLive.asObservable();
  state = "Disconnected"
 constructor(private liveService: LiveService,
    private oddService: OddWorkService) {}

  creatHubConnection(user: User){
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(this.livHubUrl + '/', {
        accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build()

    this.hubConnection.start().then(() => {
      this.state = "Finished";
       console.log("connectionId from start"+this.hubConnection.connectionId);
       this.OddListenerLive();
       this.PlaceBetListening();
       this.updateLive();
       var x : Connections ={
        Username: "",
        ConnId : []
      }
      x.Username = user.username;
      x.ConnId.push(this.hubConnection.connectionId);
      console.log("this is x")
      console.log(x.ConnId);
      this.liveService.LiveManageCon(x).subscribe(res => {
        console.log(res);
      });
      this.ListenerLive();

    }).catch(error =>console.log(error.error));

  }
  updateLive (){

    var groupName = 'LiveGameGroup';
    console.log('this is group '+ groupName);
    console.log('this the connectionId '+this.hubConnection.connectionId)
     this.hubConnection.invoke<any>('AssignToGroupz', groupName).catch(error => {
     console.error(error);
     });
  }
  updateOdds (EventId: number, MarketName: string){
    var groupName = 'event-'+EventId+'-update'+MarketName;
    //console.log('this is group '+ groupName);
    this.hubConnection.invoke<any>('RemoveFromGroup', groupName).catch(error => {
      console.error(error);
    });
  }
  OddListenerLive () {
    this.hubConnection.on("liveOddUpdate", res => {
     //console.log(res)
     this.oddsThreadSourceLive.next(res);
     this.oddsThreadLive$.pipe(take(1)).subscribe( odds => {
       this.liveService.EditedLayOdds = [];
       this.liveService.EditedBackOdds = [];
       this.liveService.LayOdds = [];
       this.liveService.BackOdds =[];
       this.oddService.checkOdd(odds);
       this.oddService.EditOdds(odds,
         this.liveService.BackOdds,
         this.liveService.LayOdds,
         this.liveService.EditedBackOdds,
         this.liveService.EditedLayOdds);

     })
   })
 }

 ListenerLive () {
    this.hubConnection.on("LiveMatchesUpdate", (res : live[])=> {
      this.liveService.liveMatches = res;
    })
  }
  PlaceBetListening(){
    this.hubConnection.on('newMatchedBet', (res : placedBet[]) => {
      //this.toastr.warning('fuck you');
      console.log('before res');
      console.log(res);
      /* for(let item of res){
        console.log('item');
        console.log(item);
        if (item.status == "Closed"){
          if (item.side == "lay"){
            this.matchService.MatchedLay.push(item);
          }else {
            this.matchService.MatchedBack.push(item);
          }
        }else {
          console.log("Ignore");
        }
      } */
      console.log('after push');
      console.log(this.getDecodedTokenId(this.userr.token));
      this.liveService.getUnMatched(this.getDecodedTokenId(this.userr.token)).subscribe(result=> {
        console.log('hi this is unMatched result');
        console.log(result);
      });
    });
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
