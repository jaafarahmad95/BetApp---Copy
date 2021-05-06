import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { authService } from '../_services/auth.Service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  registerForm: FormGroup;
  fieldTextType: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService:authService,
    private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
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
        this.router.navigateByUrl('/home');
      }
    )
  }
  Login() {

    this.authService.login(this.registerForm.value).subscribe(response => {

      var role = this.authService.getDecodedTokenRole(this.authService.getToken());
      //console.log(role);
      if (role == "Admin"){
        this.router.navigateByUrl('/admin');
      }else{
        this.router.navigateByUrl('/home');
      }

    })
  }


}
