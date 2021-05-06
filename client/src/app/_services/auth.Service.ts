import { HttpClient, HttpEvent, HttpHandler, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { nextTick } from 'process';
import { Observable, ReplaySubject } from 'rxjs';
import {map, take, tap} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { refreshToken } from '../_models/refreshToken';
import { register } from '../_models/register';
import { User } from '../_models/user';
import { LiveHubService } from './live-hub.service';
import { PresenceService } from './presence.service';
import { ScoreBoardHubService } from './score-board-hub.service';
//import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class authService implements OnDestroy {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  //jwtHelper = new JwtHelperService();
  //cachedRequests: Array<HttpRequest<any>> = [];

  private storageEventListener(event: StorageEvent) {
    if (event.storageArea === localStorage) {
      if (event.key === 'logout-event') {
        this.currentUserSource.next(null);
      }
      if (event.key === 'login-event') {
        location.reload();
      }
    }
  }

  constructor(private http: HttpClient,
    private presence: PresenceService,
    private livHub: LiveHubService,
    private SbHub: ScoreBoardHubService) {
    window.addEventListener('storage', this.storageEventListener.bind(this));
  }
  ngOnDestroy(): void {
    window.removeEventListener('storage', this.storageEventListener.bind(this));
  }
  login (model: any) {
    return this.http.post(this.baseUrl + '/auth/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          //localStorage.setItem('token',user.token);
          //this.presence.creatHubConnection(user);
          localStorage.setItem('login-event', 'login' + Math.random());
          this.currentUserSource.next(user);
        }
      })
    )
  }

  register (model: register){
    return this.http.post(this.baseUrl+ '/users/client',model).pipe(
      map( (user: User) =>{
        if (user) {
          //this.setCurrentUser(user);
          //this.presence.creatHubConnection(user);
        }
      })
    );
  }

  setCurrentUser (user: User) {
    user.roles = [];
    const roles = this.getDecodedTokenRole(this.getToken());
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    //in any point the user close the browser, move to another website or any thing else
    // SignalR automatically disconnect
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    localStorage.setItem('logout-event', 'logout' + Math.random());
    if (this.presence.state == 'Finished'){
      this.presence.stopHubConnection();
    }
    if (this.livHub.state == 'Finished'){
      this.livHub.stopHubConnection();
    }
    if (this.SbHub.state == 'Finished'){
      this.SbHub.stopHubConnection();
    }
  }
  logAsGuest(){
    return this.http.post(this.baseUrl + '/users/guests',{}).pipe(
      map((res : User) => {
        const user = res;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          //localStorage.setItem('token',user.token);
          //this.presence.creatHubConnection(user);
          localStorage.setItem('login-event', 'login' + Math.random());
          this.currentUserSource.next(user);
        }
      })
    )
  }
  getCurrentUser(): User {
    var user: User;
    this.currentUser$.pipe(take(1)).subscribe(usr => {
      user = usr;
    });
    return user;
  }

  getToken() :string {
    let usr: User;
    usr = JSON.parse(localStorage.getItem('user'));
    const tn: string = usr.token;
    return tn;
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
  getDecodedTokenUsername(token) {
    var x = JSON.parse(atob(token.split('.')[1])).username;
    return x;
  }
  getDecodedTokenRole(token){
    const userRoles = JSON.parse(atob(token.split('.')[1])).role;
    return userRoles;
  }
  /*
  public collectFailedRequest(request): void {
    this.cachedRequests.push(request);
  }
  public retryFailedRequests(): void {
    //we have the option to call this after
    //token is refreshed to fire the failed-requests
    // retry the requests. this method can
    // be called after the token is refreshed
  } */
  /*
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
  public isAuthenticated(): boolean {
    // get the token
    const token = this.getToken();
    // return a boolean reflecting
    // whether or not the token is expired
    return tokenNotExpired(token);
  }
  */
 setToken(t,rt){
  this.getCurrentUser().token = t;
  this.getCurrentUser().refreshToken = rt;
 }
 refreshToken(){
  var tk = this.getCurrentUser().token;
  var rtk = this.getCurrentUser().refreshToken;
  var x: refreshToken = {
     Token: tk,
     RefreshToken: rtk
   }
   return this.http.post(this.baseUrl + '/auth/refresh',x).pipe(
     map( (res: User) => {
      this.setToken(res.token,res.refreshToken);
     })
   );
 }
  isExpired(){
    var tk = this.getCurrentUser().token;
    const jwtToken = JSON.parse(atob(tk.split('.')[1]));
    console.log(jwtToken);
    const tokenExpired = Date.now() > (jwtToken.exp * 1000);
    if (tokenExpired){
      console.log('is Expired');
      return true;
    }
    console.log('Not Expired');
    return false;
  }
}
