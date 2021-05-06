import { DatePipe, formatDate } from '@angular/common';
import { Component, EventEmitter, HostListener, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, NgForm, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { PopLoginComponent } from '../pop-login/pop-login.component';
import { register } from '../_models/register';
import { authService } from '../_services/auth.Service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() CancelRegister = new EventEmitter ();
  xForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];
  bsModalRef: BsModalRef;
  //@ViewChild('regForm') regForm:FormGroup;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if(this.xForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private authService: authService ,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router,
    private modalService: BsModalService,
    private datePipe : DatePipe
    ) { }

  ngOnInit(): void {
    this.initilizeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  initilizeForm() {
    this.xForm = this.fb.group ({
      name: ['',Validators.required],
      lastname: ['',Validators.required],
      gender: ['male'],
      username: ['',Validators.required],
      email: ['',[Validators.required, Validators.email]],
      dateOfBirth: ['',[Validators.required]],
      city: ['',Validators.required],
      country: ['',Validators.required],
      password: ['', [ Validators.required , Validators.minLength(8) , Validators.maxLength(12) ]],
      confirmPassword: ['', [Validators.required , this.matchValues('password')]],
      personalID: ['',Validators.required],
      currency: 'TL',
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null : {isMatching: true}
    }
  }
  register(){
    var x = this.xForm.controls['dateOfBirth'].value;
    let newDate = new Date(x);

    console.log(newDate);
    var reg : register = {
      Email: this.xForm.get('email').value,
      Password: this.xForm.get('password').value,
      ConfirmPassword: this.xForm.get('confirmPassword').value,
      UserName: this.xForm.get('username').value,
      Name: this.xForm.get('name').value,
      LastName: this.xForm.get('lastname').value,
      Country: this.xForm.get('country').value,
      City: this.xForm.get('city').value,
      birthdate: newDate,
      PersonalID: this.xForm.get('personalID').value
    }
    this.authService.register(reg).subscribe( () => {
      this.toastr.success('User Registered Successfully');
    }, error => {
      console.error(error);
      this.validationErrors = error;
    },() => {
      this.xForm.reset();
      this.router.navigateByUrl('/login');
    })
  }
  cancel(){
    //this.CancelRegister.emit(false);
    this.xForm.reset();
  }
  openModal() {
    this.bsModalRef = this.modalService.show(PopLoginComponent, {class: 'modal-dialog-centered modal-sm'});
  }
}
