import { Route } from '@angular/compiler/src/core';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { PopLoginComponent } from '../pop-login/pop-login.component';
import { User } from '../_models/user';
import { authService } from '../_services/auth.Service';
import { MatchesService } from '../_services/matches.service';
import { PresenceService } from '../_services/presence.service';
import { VisibleService } from '../_services/visible.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  currentUser$: Observable<User>;
  modalRef: BsModalRef;

  constructor(public authService: authService,
    private router: Router,
    private toastr: ToastrService,
    public vs: VisibleService,
    private modalService: BsModalService,
    private presence: PresenceService) { }

  ngOnInit(): void {
    this.currentUser$ = this.authService.currentUser$;
  }

  rolecheck(){
    var role =this.authService.getDecodedTokenRole(this.authService.getCurrentUser().token);
    if (role == 'Client' || role == 'guest'){
      return true;
    }else return false;
  }

  login() {
    this.authService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/');
    },error => {
      console.log(error);
      this.toastr.error(error.error);
    })
  }
  openModal() {
    /* this is how we open a Modal Component from another component */
    this.modalRef = this.modalService.show(PopLoginComponent, {class: 'modal-dialog-centered modal-sm'});
  }
  logout() {
    this.authService.logout();
    //clear credentials
    this.model = {};
    this.router.navigateByUrl('/');
  }
  btnRegister () {
    this.router.navigateByUrl('/register');
  }

}
