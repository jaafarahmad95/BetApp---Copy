import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { scoreboard } from '../_models/scoreboard';

@Injectable({
  providedIn: 'root'
})
export class ScoreboardService {
  baseUrl = environment.apiUrl;
  board: scoreboard;
  constructor(private http: HttpClient) { }

  getScoreboard (eId: number,id){
    //var id = this.getDecodedTokenId(this.auth.getCurrentUser().token);
    return this.http.get(this.baseUrl + '/event/score/'+ id +'/'+ eId).pipe(
      map((res :scoreboard)=> {
        //this.board = res;
      })
    );
  }
  SbManageCon(body){
    return this.http.post(this.baseUrl + '/con/score',body);
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
}
