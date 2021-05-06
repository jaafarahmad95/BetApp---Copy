import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Match } from '../_models/Match';
import { Odds } from '../_models/Odds';
import { EditedOdds } from '../_models/EditedOdds';
import { Region } from '../_models/region';
import { Offer } from '../_models/Offer';
import { VisibleService } from './visible.service';
import { ToastrService } from 'ngx-toastr';
import { placedBet } from '../_models/placedBet';
import { authService } from './auth.Service';
import { User } from '../_models/user';
import { UnMatchedResult } from '../_models/UnMatchedResult';
import { Connections } from '../_models/Connections';


@Injectable({
  providedIn: 'root'
})
export class MatchesService {
  baseUrl = environment.apiUrl;
  RegionComp: Region[] = [];
  Matches: Match[]=[];
  Markets: string[];
  LayOdds: Odds[] = [];
  BackOdds: Odds[] = [];
  //Runner Odds
  EditedLayOdds: EditedOdds[] =[];
  EditedBackOdds: EditedOdds[]= [];
  //betSlip
  layBetSlip: Offer[] = [];
  backBetSlip:Offer[] = [];
  //Placedbets
  MatchedLay: placedBet[]=[];
  UnMatchedLay: UnMatchedResult[]=[];
  MatchedBack: placedBet[]=[];
  UnMatchedBack: UnMatchedResult[]=[];

  constructor(private http: HttpClient,
    private route: ActivatedRoute,
    private v : VisibleService) {

  }

  getCompetitionsInRegions() {
    return this.http.get(this.baseUrl + '/event/all').pipe(
      map ( (response: Region[]) =>{
        this.RegionComp = response;
      })
    )
  }
  getEvents(region: string,comp: string) {

    return this.http.get(this.baseUrl +'/Events/' + region + '/' + comp).pipe(
      map((response: Match[]) =>{
        this.Matches = response;
      })
    )
  }

  getMarkets(eventId: number) {
    return this.http.get(this.baseUrl + '/event/' + eventId).pipe(
      map((response: string[]) => {
        this.Markets = response;
      })
    )
  }

  getOdds(eventId:number,marketName: string,uId:string){
    return this.http.get(this.baseUrl + '/offers/'+ uId + '/' + eventId + '/' + marketName).pipe(
      map((response: Odds[]) => {
        //console.log(response);
        /* this.EditedLayOdds = [];
        this.EditedBackOdds = [];
        this.LayOdds = [];
        this.BackOdds =[]; */
        this.v.marketSuspend = false;
        //this.checkOdd(response);
        //this.EditOdds(response);
      })
    )
  }

  //for betslip array
  addLayBet(o: number,m: string,st: number,editedLayOdd: EditedOdds,eventId: number,uId: string,team:string){
    let off : Offer = {
      eventId: eventId,
      teams: team,
      userId: uId,
      marketName: m,
      side: editedLayOdd.side,
      runnerName: editedLayOdd.runnerName,
      odds: o,
      keepInPlay: editedLayOdd.keepInPlay,
      status: editedLayOdd.status,
      stake: st
    }
    this.layBetSlip.push(off);
  }
  addBackBet(o: number,m: string,st: number,editedBackOdd: EditedOdds,eventId: number,uId:string,team:string){
    let off : Offer = {
      eventId: eventId,
      teams: team,
      userId: uId,
      marketName: m,
      side: editedBackOdd.side,
      runnerName: editedBackOdd.runnerName,
      odds: o,
      keepInPlay: editedBackOdd.keepInPlay,
      status: editedBackOdd.status,
      stake: st
    }
    console.log(off);
    this.backBetSlip.push(off);
  }
  //place Bet
  placLayBet (value: Offer[] , uI:string){
    return this.http.post(this.baseUrl + '/bet/lay',value).pipe(
      map ( (ArrayOfArrays: placedBet[][]) => {
        console.log("this is Matched");
        console.log(ArrayOfArrays);
        for (let runnerArray of ArrayOfArrays){
          for (let obj of runnerArray){
            //console.log('obj.userid '+ obj.userId+ ' uId '+ uI);
            if (obj.userId === uI){
               if (obj.status == "open"){
                /* if (obj.side == "lay"){
                  this.UnMatchedLay.push(obj);
                }else {
                  this.UnMatchedBack.push(obj);
                } */
              } else {
                if(obj.side == "lay"){
                  this.MatchedLay.push(obj);
                }else {
                  this.MatchedBack.push(obj);
                }
              }
            }
          }
        }
      })
    );
  }
  placBackBet (value: Offer[],uI:string){
    return this.http.post(this.baseUrl + '/bet/back',value).pipe(
      map((ArrayOfArrays: placedBet[][]) => {
        console.log("this is Matched");
        console.log(ArrayOfArrays);
        for (let runnerArray of ArrayOfArrays){
          for (let obj of runnerArray){
            //console.log('obj.userid '+ obj.userId+ ' uId '+ uI);
            if (obj.userId === uI){
              if (obj.status === "open"){
                /* if (obj.side == "lay"){
                  this.UnMatchedLay.push(obj);
                }else {
                  this.UnMatchedBack.push(obj);
                } */
              }else {
                if (obj.side == "lay"){
                  this.MatchedLay.push(obj);
                }else {
                  this.MatchedBack.push(obj);
                }
              }
            }
          }
        }
      })
    );
  }
  ManageCon(body: Connections){
    return this.http.post(this.baseUrl + '/con/home',body);
  }
  //Delete
  /* clearUnMatchedLay(placed){
    this.http.delete(this.baseUrl + '/bet/removeLay',placed).subscribe(
      res => {
        console.log(res);
      }
    );
    this.UnMatchedLay = [];
  } */
  clearUnMatched(id){
    this.UnMatchedBack = [];
    this.UnMatchedLay = [];
    return this.http.delete(this.baseUrl + '/bet/remove/'+id);
  }
  //For Odds
  EditOdds(res:Odds[]){
    //distribute Odds
    for(let item of res){
      if (item.side === 'lay'){
        this.LayOdds.push(item);
      }else {
        this.BackOdds.push(item);
      }
    }
    //Fill Odds
    for (let item of this.LayOdds){
      var idx:number = this.OddFound(item.runnerName,this.EditedLayOdds);
      if (idx !== -1){
        this.EditedLayOdds[idx].odds.push(item.odds);
      }else {
        let x:EditedOdds={
          runnerName:'',
          marketName:'',
          side: '',
          odds : [],
          keepInPlay :false,
          status : '',
          liquidity : 0,
          visibility :'',
        };

        x.runnerName = item.runnerName;
        x.marketName = item.marketName;
        x.side = item.side;
        x.odds.push(item.odds);
        x.keepInPlay = item.keepInPlay;
        x.status = item.status;
        x.liquidity = item.liquidity;
        x.visibility = item.visibility;

        this.EditedLayOdds.push(x);

      }

    }

    for (let item of this.BackOdds){
      var idx:number = this.OddFound(item.runnerName,this.EditedBackOdds);
      if (idx !== -1){
        this.EditedBackOdds[idx].odds.push(item.odds);
      }else {
        let x:EditedOdds={
          runnerName:'',
          marketName:'',
          side: '',
          odds : [],
          keepInPlay :false,
          status : '',
          liquidity : 0,
          visibility :'',
        };

        x.runnerName = item.runnerName;
        x.marketName = item.marketName;
        x.side = item.side;
        x.odds.push(item.odds);
        x.keepInPlay = item.keepInPlay;
        x.status = item.status;
        x.liquidity = item.liquidity;
        x.visibility = item.visibility;

        this.EditedBackOdds.push(x);

      }
    }
    //replace undefined with -1
    this.ReplaceWithNegative(this.EditedLayOdds);
    this.ReplaceWithNegative(this.EditedBackOdds);
    //sort Odds
    //increasing order
    for(let item of this.EditedBackOdds){
      item.odds.sort((x, y) => (x > y ? 1 : -1));
    }
    //decreasing order
    for (let item of this.EditedLayOdds){
      item.odds.sort((x, y) => (x > y ? -1 : 1));
    }


    for (let item of this.EditedLayOdds) {
      var ones =0;
      for (let i=0;i<item.odds.length;i++){
        if (item.odds[i]===-1)
          ones++;
      }
      if (ones === 0){
        item.odds.sort((x, y) => (x > y ? +1 : -1));
      }else if (ones === 1){
        if (item.odds[0] > item.odds[1]){
          var temp = item.odds[0];
          item.odds[0] = item.odds[1];
          item.odds[1] = temp;
        }
      }
      /* console.log('this is ones ' + ones);
      console.log('below is item.odds');
      console.log(item.odds); */
    }

    //replac -1 With Undefined
    for(let item of this.EditedBackOdds){
      this.ReplaceWithEmpty(item.odds);
    }
    for (let item of this.EditedLayOdds){
      this.ReplaceWithEmpty(item.odds);
    }

  }
  checkOdd(Odds){
    var allVisible = true;
    for (let odd of Odds){
      if (odd.visibility !== "Visible"){
        console.log('there is a suspend');
        allVisible = false;
        this.v.marketSuspend = true;
      }
    }
    if (allVisible && this.v.marketSuspend == true)
      this.v.marketSuspend = false;
  }
  OddFound(name: string , arr:EditedOdds[]){
    for(let item of arr){
      if (name === item.runnerName){
        return arr.indexOf(item);
      }
    }
    return -1;
  }
  ReplaceWithNegative(arr: EditedOdds[]){
   for (let item of arr){
     while (item.odds.length<3){
       item.odds.push(-1);
     }
   }
  }
  ReplaceWithEmpty(arr: number[]){
    for (let i=0;i<arr.length;i++){
      if (arr[i] === -1){
        arr[i] = undefined;
      }
    }
  }
  /* //after update use this function to unlock market if its suspended
  checkOddsVisibility(arr: Odds[]){
    var allVisible = true;
    for (let item of arr){
      if (item.visibility!== 'Visible'){
        allVisible = false;
        console.log('not all of them visible');
      }

    }
    if (this.v.marketSuspend){
      if (allVisible)
        this.v.marketSuspend = false;
    }
  } */
  getUnMatched(id: string){
    this.UnMatchedBack =[];
    this.UnMatchedLay = [];
    let params = new HttpParams();
    if (id !== null){
      //here might be a bug in pageNumber maybe its currentPage
      params = params.append('UserID' , id);
    }
    return this.http.get(this.baseUrl + '/offers/UserBets',{observe: 'response',params}).pipe(
      map(response => {
        var x : any = response.body;
        //console.log(x);
        let Mapped: UnMatchedResult[] =[];
        x.map(r => {
          var mr: UnMatchedResult = {
            EventId : r.eventId,
            keepInPlay : r.keepInPlay,
            marketName : r.marketName,
            odds : r.odds,
            runnerName : r.runnerName,
            side : r.side,
            stake : r.stake,
            teams : r.teams,
            userId : r.userId,
            status : r.status
          }
         Mapped.push(mr);
        })
        //console.log(Mapped);
        for(let item of Mapped){
          if (item.side == "lay"){
            this.UnMatchedLay.push(item);
          }else{
            this.UnMatchedBack.push(item);
          }
        }
      })
    )
  }
}
