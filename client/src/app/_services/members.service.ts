import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { find, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { authService } from './auth.Service';
import { Accounto } from '../_models/Accounto';
import { BetHistoryResulrt } from '../_models/BetHistoryResulrt';
import { DepositeHistoryReult } from '../_models/DepositeHistoryReult';
import { WithDrawHistory } from '../_models/WithDrawHistory';
import { WithDraw } from '../_models/WithDraw';
import { userInfo } from 'os';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  member: Member;
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  memberAccount: Accounto;
  paginatedResultBetHistory: PaginatedResult<BetHistoryResulrt[]> = new PaginatedResult<BetHistoryResulrt[]>();
  paginatedResultDepositeHistory: PaginatedResult<DepositeHistoryReult[]> = new PaginatedResult<DepositeHistoryReult[]>();
  paginatedResultWithDrawHistory: PaginatedResult<WithDrawHistory[]> = new PaginatedResult<WithDrawHistory[]>();


  constructor(private http: HttpClient,private auth: authService) { }

  getMembers (page?: number,itemsPerPage?: number,search?: string,sortingStatus?: string){
    let params = new HttpParams();
    if (page!== null && itemsPerPage!==null){
      //here might be a bug in pageNumber maybe its currentPage
      params = params.append('PageNumber' , page.toString());
      params = params.append('PageSize' , itemsPerPage.toString());
      params = params.append('SearchQuery', search);
      params = params.append('SortingStatus',sortingStatus);
    }
    return this.http.get<Member[]>(this.baseUrl + '/users/all', {observe: 'response',params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('x-pagination') !== null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('x-pagination'));
        }
        return this.paginatedResult;
      })
    )
  }

  getMember (username: string){
    return this.http.get(`${this.baseUrl}/users/${username}`).pipe(
      map((res : Member)=> {
        this.member = res;
        return this.member;
      })
    );
  }


  updateMember(member: Member){
    return this.http.put(this.baseUrl + '/' ,member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  /*
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId , {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
  */
  //
  getAccount(username: string){
    return this.http.get(this.baseUrl + '/Account/' + username).pipe(
      map((res : Accounto)=> {
        console.log('this is member service getAccount')
        console.log(res);
        this.memberAccount = res;
      })
    );
  }
  /* getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  } */
  ///api/v1/Account/History/{Id}/{start}/{end}
  getbetHistory(user: User,page?: number,itemsPerPage?: number,sortingStatus?: string,searchquery?:string){
    var id = this.auth.getDecodedTokenId(user.token);
    let params = new HttpParams();
    if ( sortingStatus == undefined){
      sortingStatus = "false";
    }
    if (page!== null && itemsPerPage!==null && user!==null){
      //here might be a bug in pageNumber maybe its currentPage
      params = params.append('Userid',id.toString());
      params = params.append('PageNumber' , page.toString());
      params = params.append('PageSize' , itemsPerPage.toString());
      params = params.append('SortingStatus',sortingStatus);
     // params = params.append('Fields',field);
      params = params.append('SearchQuery',searchquery);
    }
    return this.http.get<BetHistoryResulrt[]>(this.baseUrl + '/History/Bet',{observe: 'response',params}).pipe(
      map ( response => {
        console.log(response.body);
        this.paginatedResultBetHistory.result = response.body;
        if (response.headers.get('x-pagination') !== null){
          this.paginatedResultBetHistory.pagination = JSON.parse(response.headers.get('x-pagination'));
        }
        return this.paginatedResultBetHistory;
      })
    );
  }
  getDepositeHistory(user: User,page?: number,itemsPerPage?: number,sortingStatus?: string){
    var id = this.auth.getDecodedTokenId(user.token);
    console.log(id);

    let params = new HttpParams();
    if (page!== null && itemsPerPage!==null && user!==null){
      //here might be a bug in pageNumber maybe its currentPage
      params = params.append('Userid',id.toString());
      params = params.append('PageNumber' , page.toString());
      params = params.append('PageSize' , itemsPerPage.toString());
      params = params.append('SortingStatus',sortingStatus.toString());

    }
    console.log(page)
    console.log(itemsPerPage)
    console.log(sortingStatus)

    return this.http.get<DepositeHistoryReult[]>(this.baseUrl + '/Deposits/History',{observe: 'response',params}).pipe(
      map ( response => {
        console.log(response.body);
        this.paginatedResultDepositeHistory.result = response.body;
        if (response.headers.get('x-pagination') !== null){
          this.paginatedResultDepositeHistory.pagination = JSON.parse(response.headers.get('x-pagination'));
        }
        return this.paginatedResultDepositeHistory;
      })
    )
  }
  //
  getWithdrewHistory(user: User,page?: number,itemsPerPage?: number,sortingStatus?: string){
    var id = this.auth.getDecodedTokenId(user.token);
    console.log(id);

    let params = new HttpParams();
    if (page!== null && itemsPerPage!==null && user!==null){
      //here might be a bug in pageNumber maybe its currentPage
      params = params.append('Userid',id.toString());
      params = params.append('PageNumber' , page.toString());
      params = params.append('PageSize' , itemsPerPage.toString());
      params = params.append('SortingStatus',sortingStatus.toString());

    }
    console.log(page)
    console.log(itemsPerPage)
    console.log(sortingStatus)

    return this.http.get<WithDrawHistory[]>(this.baseUrl + '/History/withdrow',{observe: 'response',params}).pipe(
      map ( response => {
        console.log(response.body);
        this.paginatedResultWithDrawHistory.result = response.body;
        if (response.headers.get('x-pagination') !== null){
          this.paginatedResultWithDrawHistory.pagination = JSON.parse(response.headers.get('x-pagination'));
        }
        return this.paginatedResultWithDrawHistory;
      })
    )
  }

  Deposite (model){
    return this.http.post(this.baseUrl + '/Deposits',model).pipe(

    )
  }
  WithDraw (model){
    return this.http.post(this.baseUrl + '/Account/withdrow',model).pipe(

    )
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
  updatePassword(id,body){
    return this.http.put(this.baseUrl + '/users/Password/'+ id,body).pipe(

    )
  }
}
