import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EditedOdds } from '../_models/EditedOdds';
import { Match } from '../_models/Match';
import { Region } from '../_models/region';
import { MatchesService } from '../_services/matches.service';
import { PresenceService } from '../_services/presence.service';
import { SelectedService } from '../_services/selected.service';
import { VisibleService } from '../_services/visible.service';
import { HubConnection } from '@microsoft/signalr';
import { authService } from '../_services/auth.Service';
import { Odds } from '../_models/Odds';
import { TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { Offer } from '../_models/Offer';
import { User } from '../_models/user';
import { FormGroup } from '@angular/forms';
import { placedBet } from '../_models/placedBet';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PopLoginComponent } from '../pop-login/pop-login.component';
import { removeUnMatched } from '../_models/removeUnMatched';
import { ScoreboardService } from '../_services/scoreboard.service';
import { UnMatchedResult } from '../_models/UnMatchedResult';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  //Tabs
  @ViewChild('betSlipTabs', {static: true}) tab: TabsetComponent;
  //const hero = HEROES.find(h => h.id === id) as Hero;
  RegionComp: Region[] = [];
  matches: Match[];
  RegionsisCollapsedarr: boolean[] = [];
  MatchesisCollapsedarr: boolean[] = [];
  markets: string[];
  //getOdds
  layOdds: EditedOdds[] = this.matchService.EditedLayOdds;
  backOdds: EditedOdds[] = this.matchService.EditedBackOdds;
  /* layOdds: EditedOdds[] = [];
  backOdds: EditedOdds[] = []; */
  private _hubConnection: HubConnection | undefined;

  betSlipLay: Offer[] = this.matchService.layBetSlip;
  betSlipBack: Offer[] = this.matchService.backBetSlip;
  //this to delete

  urlR: string;
  urlC: string;


  //PlacedBets
  matchedLay: placedBet[]=[];
  unMatchedLay: UnMatchedResult[]=[];
  matchedBack: placedBet[]=[];
  unMatchedBack: UnMatchedResult[]=[];
  //Modal
  bsModalRef: BsModalRef;

  constructor(
    private v: VisibleService,
    private matchService: MatchesService,
    private route: ActivatedRoute,
    public selected: SelectedService,
    private presence: PresenceService,
    private auth: authService,
    private router: Router,
    private toastr: ToastrService,
    private modalService: BsModalService,
  ){

  }

  ngOnInit(): void {

    /* this.matchService.search("j").subscribe(res => {
      console.log(res)
    }) */
    if (this.getdecodedToken()){
      if (this.presence.state === "Disconnected") {
        this.presence.creatHubConnection(this.auth.getCurrentUser());
      }
      let uId = this.getdecodedToken().toString();
      this.matchService.getUnMatched(uId).subscribe(
        () => {
          this.unMatchedLay = this.matchService.UnMatchedLay;
          this.unMatchedBack = this.matchService.UnMatchedLay;
        }
      );

    }

    this.v.show();
    this.urlR = this.route.snapshot.params.region;
    this.urlC = this.route.snapshot.params.competition;
    if (this.matchService.RegionComp.length === 0){
      this.getCompetitionsInRegions();
    }else {
      this.RegionComp = this.matchService.RegionComp;
      if (this.urlR === undefined){
        this.selected.initSelectedRegionCompetition(this.RegionComp[0].region,this.RegionComp[0].competition[0]);
        var url = '/'+this.RegionComp[0].region+'/'+this.RegionComp[0].competition[0];
        //here the bug of open the modal twice
        this.router.navigateByUrl(url);
        this.getMatches(this.selected.selectedRegion,this.selected.selectedCompetition);
      }else {
        this.selected.initSelectedRegionCompetition(this.urlR,this.urlC);
        this.getMatches(this.selected.selectedRegion,this.selected.selectedCompetition);
      }
    }
    //if not loggedIn show modal
    //if(!this.auth.getCurrentUser()){
    //  this.openModal();
    //}else{
    //  this.userId = this.getdecodedToken();
    //}

    //this.presence.NotiListener();
    //this.presence.updateOdds(11349087,'Match Result');
    //this.getOdds(11349087,'Match Result');
    //this.placeLayBet();

  }
  getMatchedLay(){
    return this.matchService.MatchedLay;
  }
  getMatchedLBack(){
    return this.matchService.MatchedBack;
  }
  getUnMatchedLay(){
    return this.matchService.UnMatchedLay;
  }
  getUnMatchedBack(){
    return this.matchService.UnMatchedBack;
  }
  getEditedLayOdd(){
    return this.matchService.EditedLayOdds;
  }
  getEditedBackOdd(){
    return this.matchService.EditedBackOdds;
  }
  openModal() {
    console.log('iam openModal from home component ts called now')
    this.bsModalRef = this.modalService.show(PopLoginComponent, {class: 'modal-dialog-centered modal-sm'});
  }
  getCompetitionsInRegions() {
    this.matchService.getCompetitionsInRegions().subscribe( () => {

    },error => {
      console.log(error.error);
    },
    () => {
      this.RegionComp = this.matchService.RegionComp;
      if (this.urlR === undefined){
        //console.log('urlR is undefined');
        this.selected.initSelectedRegionCompetition(this.RegionComp[0].region,this.RegionComp[0].competition[0]);
        var url = '/'+this.RegionComp[0].region+'/'+this.RegionComp[0].competition[0];
        //console.log(url);
        this.router.navigateByUrl(url);
        this.getMatches(this.selected.selectedRegion,this.selected.selectedCompetition);
      }else {
        //console.log("urlR isnt undefined and below its value");
        //console.log(this.urlR);
        this.selected.initSelectedRegionCompetition(this.urlR,this.urlC);
        this.getMatches(this.selected.selectedRegion,this.selected.selectedCompetition);
      }
      this.initRegionsCollapse();
    });


  }

  getMatches(r: string,c: string) {
    this.matchService.getEvents(r,c).subscribe(
    () => {
      this.matches = this.matchService.Matches;
    },error => {
      console.log(error.error);
    },
    () => {
      if (this.matches){
        this.selected.initSelectedMatches(this.matches[0].homeTeam,this.matches[0].awayTeam);
      }
      this.initMatchesCollapse();
    });
  }

  selectedMarkets (s: string){
    this.selected.selectedMarket = s;
  }

  sendreq(flag:boolean,eventnum:number){
    this.selected.selectedEventId = eventnum;
    if (flag){
      this.getMarkets(eventnum);
    }
  }

  getMarkets(eventNumber: number) {
    this.matchService.getMarkets(eventNumber).subscribe(
      () => {
        this.markets = this.matchService.Markets;
      }
    );
  }

  /*visiblity of markets*/
  visibleCloseOthers(market: string){
    this.selected.visibleMarkets[market] = true;
    for(var item in this.selected.visibleMarkets){
      if (item != market){
        this.selected.visibleMarkets[item] = false;
      }
    }
  }

  getOdds(eventId:number,marketName: string){
    var userId = this.auth.getDecodedTokenId(this.auth.getCurrentUser().token);
    console.log(userId);
    this.matchService.getOdds(eventId,marketName,userId).subscribe(
      () => {
        this.presence.updateOdds(eventId,marketName);
        /* this.layOdds = this.matchService.EditedLayOdds;
        this.backOdds = this.matchService.EditedBackOdds; */

      },
      error =>{
        console.error(error);
      },() => {

      }
    );
  }
  addToLayBetSlip (odd: number,market:string,stake: number,editedLayOdd: EditedOdds,eId: number){
    var homeTeam = this.selected.homeTeam;
    var awayTeam = this.selected.awayTeam;
    var team = homeTeam + ' - ' + awayTeam;
    console.log('addToLayBetSlip');
    if (odd){
      console.log('there is an odd');
     if (this.auth.getDecodedTokenId(this.auth.getToken())){
      console.log('there is an user');

      this.matchService.addLayBet(odd,market,stake,editedLayOdd,eId,this.auth.getDecodedTokenId(this.auth.getToken()),team);
     }

    }
  }

  addToBackBetSlip (odd: number,market:string,stake: number,editedBackOdd: EditedOdds,eId: number){
    var homeTeam = this.selected.homeTeam;
    console.log(homeTeam)
    var awayTeam = this.selected.awayTeam;
    console.log(awayTeam)

    var team = homeTeam + ' - ' + awayTeam;
    console.log(team)

    if (odd){
      if (this.auth.getDecodedTokenId(this.auth.getToken())){
        this.matchService.addBackBet(odd,market,stake,editedBackOdd,eId,this.auth.getDecodedTokenId(this.auth.getToken()),team);
      }
    }
  }

  backAll(m:string){
    for (let item of this.getEditedBackOdd()){
      this.addToBackBetSlip(item.odds[(item.odds.length)-1],m,0,item,this.selected.selectedEventId);
    }
  }

  layAll(m:string){
    for (let item of this.getEditedLayOdd()){
      this.addToLayBetSlip(item.odds[0],m,0,item,this.selected.selectedEventId);
    }
  }

  /*for collapse functionality*/
  initRegionsCollapse() {
    this.RegionsisCollapsedarr.length = this.RegionComp.length;
    for (let i = 0; i < this.RegionsisCollapsedarr.length; i++) {
      this.RegionsisCollapsedarr[i] = false;
    }
  }
  initMatchesCollapse() {
    this.MatchesisCollapsedarr.length = this.matches.length;
    for (let i = 0; i < this.MatchesisCollapsedarr.length; i++) {
      this.MatchesisCollapsedarr[i] = false;
    }
  }
  collapseCloseOthers (arr,idx) {
    for (let i=0 ; i<arr.length;i++){
      if (i !== idx){
        arr[i] = false;
      }
    }
  }
  /* bet slip section*/
  //For remove Offer in betslip
  onCloseClick(index, arr){
    arr.splice(index,1);
  }
  //for suspend section true if market suspend else false
  getSuspendStatus(): boolean{
    return this.v.marketSuspend;
  }
  //place bet button
  PlaceClick(){
    //this.presence.PlaceBetListening();
    var role = this.auth.getDecodedTokenRole(this.auth.getToken());
    if (role != "Admin"){
      if (!this.v.warnBetSlip){
        if(this.betSlipLay.length > 0){
          this.matchService.placLayBet(this.betSlipLay,this.auth.getDecodedTokenId(this.auth.getToken())).subscribe(res => {
            this.matchedLay = this.matchService.MatchedLay;
            this.unMatchedLay = this.matchService.UnMatchedLay;
          },error => {
            console.error(error);
          }, () => {
            this.toastr.success('Successfully Placed your Lay Bets');
            this.clearBetSlip();
          })
        }
        if(this.betSlipBack.length > 0){
          this.matchService.placBackBet(this.betSlipBack,this.auth.getDecodedTokenId(this.auth.getToken())).subscribe(res => {
            this.matchedBack = this.matchService.MatchedBack;
            this.unMatchedBack = this.matchService.UnMatchedBack;
          },error => {
            console.error(error);
          }, () => {
            this.toastr.success('Successfully Placed your Back Bets');
            this.clearBetSlip();
          })
        }
        this.tab.tabs[1].active = true;
      }
    }else {
      this.toastr.error("You cannt bet as Admin");
    }
  }
  activePlaceBets(){
    this.tab.tabs[0].active = true;
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
  clearBetSlip(){
    this.betSlipLay.splice(0,this.betSlipLay.length);
    this.betSlipBack.splice(0,this.betSlipBack.length);
  }

  clearPlacedUnMatched(){

    if (this.matchService.UnMatchedLay.length + this.matchService.UnMatchedBack.length > 0){
      /* var x: removeUnMatched[] =[];
      for (let item of this.matchService.UnMatchedLay){
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
      this.matchService.clearUnMatched(this.auth.getDecodedTokenId(this.auth.getToken())).subscribe(
        res => {
          this.toastr.success(res.toString());
        }
      );
      this.unMatchedLay = [];
      this.unMatchedBack = [];
    }

   /*  if (this.matchService.UnMatchedBack.length > 0){
      var x1: removeUnMatched[] =[];
      for (let item of this.matchService.UnMatchedBack){
        var y1: removeUnMatched = {
          userid: item.userId,
          odds: item.odds,
          eventid: item.EventId,
          MarketName: item.marketName,
          Side: item.side,
          RunnerName: item.runnerName
        }
        console.log(y1);
        x1.push(y1);
        console.log(x1);
      }
      this.matchService.clearUnMatched(x1);
      this.unMatchedBack = [];
    } */
  }

  //Floor values
  RoundValue(num){
    var fn = Math.floor(num* 100) / 100;
    return fn;
  }
  //getDecoded Token
  getdecodedToken(){
    if (this.auth.getCurrentUser()){
      var tk = this.auth.getCurrentUser().token;
      return this.auth.getDecodedTokenId(tk);
    }
  }
}
