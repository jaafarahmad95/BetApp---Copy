import { HttpClient } from '@angular/common/http';
import { ConvertActionBindingResult } from '@angular/compiler/src/compiler_util/expression_converter';
import { Injectable } from '@angular/core';
import { HubConnection,HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Connections } from '../_models/Connections';
import { Odds } from '../_models/Odds';
import { Offer } from '../_models/Offer';
import { placedBet } from '../_models/placedBet';
import { TaskSignalR } from '../_models/TaskSignalR';
import { User } from '../_models/user';
import { authService } from './auth.Service';
import { LiveService } from './live.service';
import { MatchesService } from './matches.service';
import { OddWorkService } from './odd-work.service';
import { VisibleService } from './visible.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  userr: User;
  private hubConnection: HubConnection;
  private oddsThreadSource = new BehaviorSubject<Odds[]>([]);
  oddsThread$ = this.oddsThreadSource.asObservable();
  private oddsThreadSourceLive = new BehaviorSubject<Odds[]>([]);
  oddsThreadLive$ = this.oddsThreadSource.asObservable();
  state = "Disconnected"
  constructor(private toastr: ToastrService,
    private http: HttpClient,
    private matchService: MatchesService,
    private v : VisibleService) {

  }

  //create a method create a hub connection to the user after he login and authorized
  //we will use the user to send our jwt token when make this connection to the presenceHub
  //we cannot use interceptor and these are gonna no longer be an http requests this gonna be
  //deiffrent and typcially gonna be using webSockets wich has no support for authentication Headers.

  creatHubConnection(user: User){
    this.userr = user;
    this.hubConnection = new HubConnectionBuilder()
    //same name as endPoints that we use for our hub
      .withUrl(this.hubUrl + '/', {
        //user.token that what we gonna returning
        accessTokenFactory: () => user.token
      })
      //this if the user have a connection problem our client will automaticially reconnect to our hub
      .withAutomaticReconnect()
      //then build and these gonna take care of creating the hub connection
      //and what we need next is to start the hub connection
      .build()

    this.hubConnection
      .start()
      .then(() => {
        //console.log('connection started from pres');
        //console.log("connectionId from start"+this.hubConnection.connectionId)
        this.state = "Finished";
        this.OddListener();
        this.PlaceBetListening();
        console.log('this is from start');
        console.log(this.hubConnection.connectionId);

        /* var s: string[];
        s.push(this.hubConnection.connectionId);
        */
        var x : Connections ={
          Username: "",
          ConnId : []
        }
        x.Username = user.username;
        x.ConnId.push(this.hubConnection.connectionId);
        console.log("this is x")
        console.log(x.ConnId);
        this.matchService.ManageCon(x).subscribe(res => {
          //console.log(res)
        },error => {
          console.log(error);
        });
      })

      //and we catch any error
      .catch(error =>console.log(error.error));

    //and what we do next is listen for the server events , listen to
    //user isOnline Methods and User on Offline methods
    //first param is the method name and the second is what we get from this method
    /* this.hubConnection.on('liveGameUpdate', odds => {
      this.oddsThreadSource.next(odds);
      this.oddsThread$.pipe(take(1)).subscribe( odds => {
        this.ms.EditOdds(odds);
        //this.ms.checkOddsVisibility(odds);
      })
    }) */
    /* this.hubConnection.on('', odds => {
      this.oddsThreadSource.next(odds);
      this.oddsThread$.pipe(take(1)).subscribe( odds => {
        this.matchService.EditOdds(odds);
        this.matchService.checkOddsVisibility(odds);
      })
    }) */


    //and be carefull with these because they will have to be exactly match what
    //we call these methods in API PresenceHub.cs


  }
  getConnection(){
    return this.hubConnection;
  }

  OddListener () {
    /* this.hubConnection.on('', res => {
      this.toastr.warning(' Iam Listining');
      console.log(res);
    }) */

     this.hubConnection.on("liveGameUpdate", res => {
      //console.log(res);
      this.oddsThreadSource.next(res);
      this.oddsThread$.pipe(take(1)).subscribe( odds => {
        this.matchService.checkOdd(odds);
        //this.matchService.checkOddsVisibility(odds);
        this.matchService.LayOdds = [];
        this.matchService.BackOdds =[];
        this.matchService.EditedLayOdds = [];
        this.matchService.EditedBackOdds = [];
        this.matchService.EditOdds(odds);



        /* console.log(this.matchService.EditedLayOdds);
        console.log(this.matchService.EditedBackOdds); */
        //this.matchService.checkOddsVisibility(odds);
      })
    })
  }

  PlaceBetListening(){
    this.hubConnection.on('newMatchedBet', (res : placedBet[]) => {
      //this.toastr.success('Your bet is Matched');
      console.log('before res');
      console.log(res);

      /* for(let item of res){
        //console.log('item');
        //console.log(item);
        if (item.status == "Closed"){
          if (item.side == "lay"){
            this.matchService.MatchedLay.push(item);
          }else if (item.side == "back") {
            this.matchService.MatchedBack.push(item);
          }
        }else {
          console.log("Ignore");
        }
      } */
      console.log('after push');
      console.log(this.getDecodedTokenId(this.userr.token));
      this.matchService.getUnMatched(this.getDecodedTokenId(this.userr.token)).subscribe(result=> {
        console.log('hi this is unMatched result');
        console.log(result);
      });
    });
  }
  NotiListener () {
    /* this.hubConnection.on('RecieveNotification', res => {
      this.toastr.warning(' Iam Listining');
      console.log(res);
    }) */
  }
  updateOdds (EventId: number, MarketName: string){
    /* let pr: TaskSignalR = {
      eventId: EventId,
      marketName: MarketName
    }
    this.hubConnection.invoke('UpdateTasks', pr).catch(err => {
      console.error(err);
    }); */

    var groupName = 'event-'+EventId+'-update'+MarketName;
    //console.log(groupName);
    //console.log('this is group '+ groupName);
    this.hubConnection.invoke<any>('RemoveFromGroup', groupName).catch(error => {
      console.error(error);
    });
  }

  stopHubConnection() {
    this.state = "Disconnected"
    this.hubConnection.stop().catch(error => console.log(error));
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
}
