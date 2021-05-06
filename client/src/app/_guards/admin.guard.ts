import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { authService } from '../_services/auth.Service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private auth: authService,
    private toastr:ToastrService,){}
    //Client,Admin
    //Adminuser: Admin,Disc123!
  canActivate(): Observable<boolean>  {
    var role = this.auth.getDecodedTokenRole(this.auth.getToken());
    return this.auth.currentUser$.pipe(
      map(user => {
        console.log(user);
        if (role == 'Admin') {
          return true;
        }
        this.toastr.error('You cannt enter this area');
      })
    );
  }

}
