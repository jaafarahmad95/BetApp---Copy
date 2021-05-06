import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { authService } from '../_services/auth.Service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private authServices:authService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User;

    this.authServices.currentUser$.pipe(take(1)).subscribe(user => currentUser = user);

    if (currentUser){
      console.log("a request is sent");
      request = request.clone({
        setHeaders: {
          //Authorization: `Bearer ${currentUser.token}`,
          Authorization: `Bearer ${currentUser.token}`,
          //Authorization: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqYWFmYXIiLCJqdGkiOiIyYTkyMDM3OS0wMjU5LTQxODQtOWRkZS05YTdlZjY2YzhhOGIiLCJpZCI6IjYwMjE0NmJlMTkyZDZlMjU1Y2QzYzRiOSIsIm5hbWUiOiJqYWFmYXIiLCJyb2xlIjoiQ2xpZW50IiwibmJmIjoxNjE4NzQ1NTI4LCJleHAiOjE2MjQ5NjYzMjgsImlhdCI6MTYxODc0NTUyOH0.8rgg-yYEbjVJzc_ffXRH1LICnuEfO9AiDzcdWB6ylQY`,
        },
      });
    }

    return next.handle(request);
  }
}
