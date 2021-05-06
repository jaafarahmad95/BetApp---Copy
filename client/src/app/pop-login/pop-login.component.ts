import { Component, OnInit ,TemplateRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../_services/auth.Service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { config } from 'rxjs';

@Component({
  selector: 'app-pop-login',
  templateUrl: './pop-login.component.html',
  styleUrls: ['./pop-login.component.css']
})
export class PopLoginComponent implements OnInit {
  registerForm: FormGroup;
  fieldTextType: boolean;


  constructor(private modalService: BsModalService,
    private modalRef: BsModalRef,
    private fb: FormBuilder,
    private authService:authService,
    private router: Router,
    private toastr: ToastrService) {}

  ngOnInit() {
    this.initilizeForm();
  }

  initilizeForm () {
    this.registerForm = this.fb.group({
      username: ['',Validators.required],
      password: ['', [ Validators.required , Validators.minLength(8) , Validators.maxLength(12) ]],
    });
  }

  toggleFieldTextType() {
    this.fieldTextType = !this.fieldTextType;
  }
  LogAsGuest(){
    this.authService.logAsGuest().subscribe(
      res => {
        this.hide();
        this.router.navigateByUrl('/home');
      }
    )
  }
  Login() {
    this.authService.login(this.registerForm.value).subscribe(response => {
      this.hide();
      this.router.navigateByUrl('/home');
    })
  }

  /*openModal() {
    this.modalRef = this.modalService.show(PopLoginComponent, {class: 'modal-dialog-centered modal-sm'});
  }*/

  hide () {
    this.modalRef.hide();

  }
}
