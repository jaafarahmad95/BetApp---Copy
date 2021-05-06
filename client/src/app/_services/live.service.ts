import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { EditedOdds } from '../_models/EditedOdds';
import { live } from '../_models/live';
import { Match } from '../_models/Match';
import { Odds } from '../_models/Odds';
import { Offer } from '../_models/Offer';
import { placedBet } from '../_models/placedBet';
import { UnMatchedResult } from '../_models/UnMatchedResult';
import { OddWorkService } from './odd-work.service';
import { VisibleService } from './visible.service';

@Injectable({
  providedIn: 'root'
})
export class LiveService {
  baseUrl = environment.apiUrl;
  liveMatches: live[] = [];
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
    private v : VisibleService,
    private oddService: OddWorkService,
    ) { }
  LiveManageCon(body){
      return this.http.post(this.baseUrl + '/con/live',body).pipe(
        map(()=> {
          console.log("this is from pipe")
          console.log(body)
        })
      );
  }

  getLive(){
    return this.http.get(this.baseUrl + '/events/live').pipe(
      map((res: live[])=>{
        this.liveMatches = res;
      })
    );
  }

  getMarkets(eventId: number) {
    return this.http.get(this.baseUrl + '/event/' + eventId).pipe(
      map((response: string[]) => {
        this.Markets = response;
      })
    )
  }

  getOdds(eventId:number,marketName: string,uId:string){
    console.log(marketName);
    return this.http.get(this.baseUrl + '/offers/'+ uId + '/' +eventId + '/' + marketName).pipe(
      map((response: Odds[]) => {
       // console.log(response);
        //this.EditedLayOdds = [];
        //this.EditedBackOdds = [];
        //this.LayOdds = [];
        //this.BackOdds =[];
        this.v.marketSuspend = false;
        //this.oddService.checkOdd(response);
        /* this.oddService.EditOdds(response,
          this.BackOdds,
          this.LayOdds,
          this.EditedBackOdds,
          this.EditedLayOdds); */
      })
    )
  }

  //betSlip
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
    this.backBetSlip.push(off);
  }
  //place Bet
  placLayBet (value: Offer[] , uI:string){
    return this.http.post(this.baseUrl + '/bet/lay',value).pipe(
      map ( (ArrayOfArrays: placedBet[][]) => {
        console.log('this is lay');
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
              }else {
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
        console.log('this is back');
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
  //Delete
  clearUnMatched(id){
    this.UnMatchedBack = [];
    this.UnMatchedLay = [];
    return this.http.delete(this.baseUrl + '/bet/remove/'+id);
  }
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
        console.log(x);
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
        console.log(Mapped);
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
