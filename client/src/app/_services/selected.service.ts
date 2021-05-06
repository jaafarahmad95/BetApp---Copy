import { Injectable } from '@angular/core';
import { Match } from '../_models/Match';

@Injectable({
  providedIn: 'root'
})
export class SelectedService {
  selectedRegion: string='';
  selectedCompetition: string='';
  selectedMatchObject: Match;
  homeTeam: string ="Team1";
  awayTeam: string ="Team2";
  selectedMarket: string="";
  selectedEventId: number=0;

  visibleMarkets = {
    'Match Result': false,
    'Both Teams to Score': false,
    'Double Chance': false,
    'Total Goals': false
  };

  constructor() { }

  initSelectedRegionCompetition (r,c){
    this.selectedRegion = r;
    this.selectedCompetition = c;
  }
  setSelectedRegion (r: string){
    console.log('setRegion '+r);
    this.selectedRegion = r;

  }
  getSelectedRegion(): string {
    //console.log('getRegion ' + this.selectedRegion);
    return this.selectedRegion;
  }
  setSelectedCompetitions(c: string){
    console.log('setComp '+c);
    this.selectedCompetition = c;
  }
  getSelectedCompetitions(): string {
    //console.log('getComp '+this.selectedCompetition);
    return this.selectedCompetition;
  }
  initSelectedMatches(h,a){
    //console.log('initSelectedMatches '+h+ a);
    this.homeTeam = h;
    this.awayTeam = a;
  }
  setSelectedMatch(home,away){
    //console.log('setSelectedMatch '+home+ away);
    this.homeTeam = home;
    this.awayTeam = away;
  }
  getSelectedMatch(): string {
    //console.log('getSelectedMatch '+this.homeTeam + " VS " + this.awayTeam);
    return this.homeTeam + " VS " + this.awayTeam;
  }
  initSelectedMarkets(){
    for (let i in this.visibleMarkets){
      this.visibleMarkets[i] = false;
    }
  }
  visibleCloseOthers(market: string){
    this.visibleMarkets[market] = true;
    for(var item in this.visibleMarkets){
      if (item != market){
        this.visibleMarkets[item] = false;
      }
    }
  }
}
