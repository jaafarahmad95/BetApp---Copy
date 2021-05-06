import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { authService } from '../_services/auth.Service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor (private authService: authService , private toastr: ToastrService,private router : Router){}

  canActivate(): Observable<boolean>{
    return this.authService.currentUser$.pipe(
      map(user => {
        if (user) return true;
        this.toastr.error('You are not authenticated');
        this.router.navigateByUrl('/home');
      })
    )
  }

 /*  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }
    this.toastr.error('You shall not pass!!!');
    this.router.navigate(['/home']);
    return false;
  } */
}

