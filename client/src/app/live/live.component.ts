import { Component, OnInit, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { EditedOdds } from '../_models/EditedOdds';
import { live } from '../_models/live';
import { Offer } from '../_models/Offer';
import { placedBet } from '../_models/placedBet';
import { removeUnMatched } from '../_models/removeUnMatched';
import { scoreboard } from '../_models/scoreboard';
import { UnMatchedResult } from '../_models/UnMatchedResult';
import { authService } from '../_services/auth.Service';
import { CollapsedService } from '../_services/collapsed.service';
import { LiveHubService } from '../_services/live-hub.service';
import { LiveService } from '../_services/live.service';
import { PresenceService } from '../_services/presence.service';
import { ScoreBoardHubService } from '../_services/score-board-hub.service';
import { ScoreboardService } from '../_services/scoreboard.service';
import { SelectedService } from '../_services/selected.service';
import { VisibleService } from '../_services/visible.service';

@Component({
  selector: 'app-live',
  templateUrl: './live.component.html',
  styleUrls: ['./live.component.css']
})
export class LiveComponent implements OnInit {
  @ViewChild('betSlipTabs', {static: true}) tab: TabsetComponent;
  livegames:live[] = [];
  liveMarkets: string[]=[];
  layOdds: EditedOdds[] = this.liveService.EditedLayOdds;
  backOdds: EditedOdds[] = this.liveService.EditedBackOdds;
  //betSlip
  betSlipLay: Offer[] = this.liveService.layBetSlip;
  betSlipBack: Offer[] = this.liveService.backBetSlip;
  //PlacedBets
  matchedLay: placedBet[]=[];
  unMatchedLay: UnMatchedResult[]=[];
  matchedBack: placedBet[]=[];
  unMatchedBack: UnMatchedResult[]=[];
  //scoreboard
  scoreboard: scoreboard;
  constructor(private liveService: LiveService,
    public collapse:CollapsedService,
    public selected: SelectedService,
    private v : VisibleService,
    private auth: authService,
    private toastr: ToastrService,
    private sboard: ScoreboardService,
    private liveHub: LiveHubService,
    private sboardHub: ScoreBoardHubService
    ) { }

  ngOnInit(): void {
    this.Live();
    this.selected.initSelectedMarkets();
    if (this.liveHub.state === "Disconnected") {
      this.liveHub.creatHubConnection(this.auth.getCurrentUser());
    }
    if (this.sboardHub.state === "Disconnected") {
      this.sboardHub.creatHubConnection(this.auth.getCurrentUser());
    }

    let uId = this.getdecodedToken().toString();

    this.liveService.getUnMatched(uId).subscribe(
      () => {
        this.unMatchedLay = this.liveService.UnMatchedLay;
        this.unMatchedBack = this.liveService.UnMatchedLay;
      }
    );
  }
  getMatchedLay(){
    return this.liveService.MatchedLay;
  }
  getMatchedLBack(){
    return this.liveService.MatchedBack;
  }
  getUnMatchedLay(){
    return this.liveService.UnMatchedLay;
  }
  getUnMatchedBack(){
    return this.liveService.UnMatchedBack;
  }
  getliveMatches(){
    return this.liveService.liveMatches;
  }
  //scoreboard
  getBoard(){
    return this.sboard.board;
  }
  //get Live Matches
  Live(){
    this.liveService.getLive().subscribe(
      () => {
        this.livegames = this.liveService.liveMatches;
      },error => {
        console.error(error);
      }, () =>{
        this.collapse.initCollapse(this.collapse.liveCollapseArr,this.livegames.length);
      }
    )
  }
  //getMarkets
  getMarket(eId:number){
    this.liveService.getMarkets(eId).subscribe(
      () => {
        this.liveMarkets = this.liveService.Markets;
      }
    )
  }
  getOdds(eId: number,m: string){
    var userId = this.auth.getDecodedTokenId(this.auth.getCurrentUser().token);
    this.liveService.getOdds(eId,m,userId).subscribe(
      () => {
        this.liveHub.updateOdds(eId,m);
        //this.layOdds = this.liveService.EditedLayOdds;
        //this.backOdds = this.liveService.EditedBackOdds;
      },error => {
        console.error(error);
      }, () => {

      }
    );
  }

  getEditedLayOdd(){
    return this.liveService.EditedLayOdds;
  }
  getEditedBackOdd(){
    return this.liveService.EditedBackOdds;
  }
  //BetSlip
  addToLayBetSlip (odd: number,market:string,stake: number,editedLayOdd: EditedOdds,eId: number){
    if (odd){
     if (this.auth.getDecodedTokenId(this.auth.getCurrentUser().token)){
      this.liveService.addLayBet(odd,market,stake,editedLayOdd,eId,this.auth.getDecodedTokenId(this.auth.getCurrentUser().token),this.selected.homeTeam+ ' - ' +this.selected.awayTeam);
     }

    }
  }
  addToBackBetSlip (odd: number,market:string,stake: number,editedBackOdd: EditedOdds,eId: number){
    if (odd){
      if (this.auth.getDecodedTokenId(this.auth.getCurrentUser().token)){
        this.liveService.addBackBet(odd,market,stake,editedBackOdd,eId,this.auth.getDecodedTokenId(this.auth.getCurrentUser().token),this.selected.homeTeam+ ' - ' +this.selected.awayTeam);
      }
    }
  }
  //scoreboard
  getScoreBorad(eId: number) {
    var id = this.auth.getDecodedTokenId(this.auth.getCurrentUser().token);
    console.log(id)
    this.sboard.getScoreboard(eId,id).subscribe(
      ()=> {},
      error=> {console.error(error);},
      ()=> {
        //this.scoreboard = this.sboard.board;
      }
    );
  }
  closeScoreBoard(){
    this.collapse.closeScoreBoard();
  }
  backAll(m:string){
    for (let item of this.backOdds){
      this.addToBackBetSlip(item.odds[(item.odds.length)-1],m,0,item,this.selected.selectedEventId);
    }
  }

  layAll(m:string){
    for (let item of this.layOdds){
      this.addToLayBetSlip(item.odds[0],m,0,item,this.selected.selectedEventId);
    }
  }

  //for visibilty of market if its suspended or not
  getSuspendStatus(): boolean{
    return this.v.marketSuspend;
  }
  getMarketStatus(market){
    return this.selected.visibleMarkets[market];
  }
  //for tabset
  activePlaceBets(){
    this.tab.tabs[0].active = true;
  }
  //Round results
  RoundValue(num){
    var fn = Math.floor(num* 100) / 100;
    return fn;
  }
  //place bet button
  PlaceClick(){
    if (!this.v.warnBetSlip){
      if(this.betSlipLay.length > 0){
        this.liveService.placLayBet(this.betSlipLay,this.auth.getDecodedTokenId(this.auth.getCurrentUser().token)).subscribe(res => {
          this.matchedLay = this.liveService.MatchedLay;
          this.unMatchedLay = this.liveService.UnMatchedLay;
        },error => {
          console.error(error);
        }, () => {
          this.toastr.success('Successfully Placed your Lay Bets');
          this.clearBetSlip();
        })
      }
      if(this.betSlipBack.length > 0){
        this.liveService.placBackBet(this.betSlipBack,this.auth.getDecodedTokenId(this.auth.getCurrentUser().token)).subscribe(res => {
          this.matchedBack = this.liveService.MatchedBack;
          this.unMatchedBack = this.liveService.UnMatchedBack;
        },error => {
          console.error(error);
        }, () => {
          this.toastr.success('Successfully Placed your Back Bets');
          this.clearBetSlip();
        })
      }
      this.tab.tabs[1].active = true;
    }

  }
  clearPlacedUnMatched(){

    if (this.liveService.UnMatchedLay.length + this.liveService.UnMatchedBack.length > 0){
      /* var x: removeUnMatched[] =[];
      for (let item of this.liveService.UnMatchedLay){
          var y: removeUnMatched = {
            userid: item.userId,
            odds: item.odds,
            eventid: item.EventId,
            MarketName: item.marketName,
            Side: item.side,
            RunnerName: item.runnerName
          }
          console.log('this is y ');
          console.log(y);
          x.push(y);
          console.log('this is x ');
          console.log(x);
      } */
      this.liveService.clearUnMatched(this.auth.getDecodedTokenId(this.auth.getToken())).subscribe(
        res => {
          this.toastr.success(res.toString());
        }
      );
      this.unMatchedLay = [];
      this.unMatchedBack = [];
    }

    /* if (this.liveService.UnMatchedBack.length > 0){
      var x1: removeUnMatched[] =[];
      for (let item of this.liveService.UnMatchedBack){
        var y1: removeUnMatched = {
          userid: item.userId,
          odds: item.odds,
          eventid: item.EventId,
          MarketName: item.marketName,
          Side: item.side,
          RunnerName: item.runnerName
        }
        console.log('this is y1 ');
        console.log(y1);
        x1.push(y1);
        console.log('this is x1 ');
        console.log(x1);
      }
      this.liveService.clearUnMatched(x1);

    } */
  }
  placeBetCheck(){
    this.v.warnBetSlip = false;
    for (let item of this.betSlipLay){
      if (item.stake == 0){
        //this.toastr.error('Stake Cannot be 0');
        //return false;
        this.v.warnBetSlip = true;
      }
    }
    for (let item of this.betSlipBack){
      if (item.stake == 0){
        //this.toastr.error('Stake Cannot be 0');
        //return false;
        this.v.warnBetSlip = true;
      }
    }
    for (let bitem of this.betSlipBack){
      for (let litem of this.betSlipLay){
        if (bitem.runnerName === litem.runnerName){
          if (bitem.odds < litem.odds){
            //this.toastr.error('lay Odds cannot be bigger than back Odds for the same match');
            //return false;
            this.v.warnBetSlip = true;
          }
        }
      }
    }
  }
  getWarning(){
    return this.v.warnBetSlip;
  }
  //remove from betSlip
  onCloseClick(index, arr){
    arr.splice(index,1);
  }
  clearBetSlip(){
    this.betSlipLay.splice(0,this.betSlipLay.length);
    this.betSlipBack.splice(0,this.betSlipBack.length);
  }
  getdecodedToken(){
    var tk = this.auth.getCurrentUser().token;
    if(tk){
      return this.auth.getDecodedTokenId(tk);
    }
  }
}
