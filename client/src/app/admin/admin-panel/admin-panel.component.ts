import { Component, HostListener, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import * as EventEmitter from 'events';
import { ToastrService } from 'ngx-toastr';
import { CurrencyUpdate, UpdateCurrencyDto } from 'src/app/_models/CurrencyUpdate';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserState } from 'src/app/_models/UserState';
import { AdminService } from 'src/app/_services/admin.service';
import { authService } from 'src/app/_services/auth.Service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
  xForm: FormGroup;
  validationErrors: string[] = [];
  members: Member[];
  //modifiedMembers: Member[] = [];
  pagination: Pagination;
  SearchQuery="";
  pageNumber = 1;
  pageSize = 5;
  SortingStatus="Ascending";
  //
  AnalyzeName: string="";
  userAnalyze: UserState;
  //
  chooseCurrency;
  currencyValue;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if(this.xForm.dirty) {
      $event.returnValue = true;
    }
  }
  constructor(private authService: authService ,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router,
    private adminService: AdminService,
    private memberService: MembersService,) { }

  ngOnInit(): void {
    this.initilizeForm();
    this.getUsers(this.SearchQuery);

  }
  Search(){
    this.getUsers(this.SearchQuery);
  }
  getUsers(search: string){
    this.memberService.getMembers(this.pageNumber,this.pageSize,search,this.SortingStatus).subscribe(
      res => {
        this.members = res.result;
        this.initilizeDate(res.result);
        this.pagination = res.pagination;
      }
    );
  }
  initilizeForm() {
    this.xForm = this.fb.group ({
      name: ['',Validators.required],
      lastname: ['',Validators.required],
      username: ['',Validators.required],
      email: ['',[Validators.required, Validators.email]],
      password: ['', [ Validators.required , Validators.minLength(8) , Validators.maxLength(12) ]],
      confirmPassword: ['', [Validators.required , this.matchValues('password')]],
    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null : {isMatching: true}
    }
  }
  cancel(){
    this.xForm.reset();
  }
  creatAdmin(){
    console.log(this.xForm.value);
    this.adminService.CreateAdmin(this.xForm.value).subscribe( () => {
      this.toastr.success('Admin Created Successfully');
    }, error => {
      this.validationErrors = error;
    },() => {
      this.xForm.reset();
      this.router.navigateByUrl('/admin');
    })
  }
  getUserState(){
    this.adminService.getUserState(this.AnalyzeName).subscribe(
      res => {
        console.log(res);
        this.userAnalyze = res;
      }
    )
  }
  updateCurrency(){
    var cchooseCurrency: number = +this.chooseCurrency;
    var cvalue : number = +this.currencyValue;
    var inner: UpdateCurrencyDto = {
      CurrenceyId : cchooseCurrency,
      value : cvalue
    }

    var id : string = this.authService.getDecodedTokenId(this.authService.getCurrentUser().token);
    var CurrencyList1:UpdateCurrencyDto[]=[];
    CurrencyList1.push(inner);
    var x: CurrencyUpdate = {
      userId : id,
      CurrencyList : CurrencyList1
    }
    return this.adminService.UpdateCurrency(x).subscribe(

    )
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.getUsers(this.SearchQuery);
    /* this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers(); */
  }
  initilizeDate(arr){
    for (let item of arr){
      console.log(item.birthdate);
    }
  }
}
