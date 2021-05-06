import { Injectable } from '@angular/core';
import { EditedOdds } from '../_models/EditedOdds';
import { Odds } from '../_models/Odds';
import { VisibleService } from './visible.service';

@Injectable({
  providedIn: 'root'
})
export class OddWorkService {

  constructor(private v: VisibleService) { }

  EditOdds(res:Odds[],back:Odds[],lay:Odds[],Eback: EditedOdds[],Elay: EditedOdds[]){
    //distribute Odds
    for(let item of res){
      if (item.side === 'lay'){
        lay.push(item);
      }else {
        back.push(item);
      }
    }
    //Fill Odds
    for (let item of lay){
      var idx:number = this.OddFound(item.runnerName,Elay);
      if (idx !== -1){
        Elay[idx].odds.push(item.odds);
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

        Elay.push(x);

      }

    }

    for (let item of back){
      var idx:number = this.OddFound(item.runnerName,Eback);
      if (idx !== -1){
        Eback[idx].odds.push(item.odds);
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

        Eback.push(x);

      }
    }
    //replace undefined with -1
    this.ReplaceWithNegative(Elay);
    this.ReplaceWithNegative(Eback);
    //sort Odds
    //increasing order
    for(let item of Eback){
      item.odds.sort((x, y) => (x > y ? 1 : -1));
    }
    //decreasing order
    for (let item of Elay){
      item.odds.sort((x, y) => (x > y ? -1 : 1));
    }

    for (let item of Elay) {
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

    }

    //replac -1 With Undefined
    for(let item of Eback){
      this.ReplaceWithEmpty(item.odds);
    }
    for (let item of Elay){
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

}
