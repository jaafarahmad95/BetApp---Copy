import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { UserState } from '../_models/UserState';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  userState: UserState;
  constructor(private http: HttpClient) { }

 /*  getAllUser(){
    return this.http.get(this.baseUrl + '/users/all');
  } */

  CreateAdmin(model){
    return this.http.post(this.baseUrl + '/users/Admin',model);
  }
  getUserState(UserName){
    return this.http.get(this.baseUrl + '/user/analyze/'+UserName).pipe(
      map((res: UserState) => {
        this.userState = res;
        return this.userState;
      })
    )
  }
  UpdateCurrency(body){
    return this.http.put(this.baseUrl+ '/currency/update',body).pipe(
      map(res => {
        console.log(res);
      })
    )
  }
}
