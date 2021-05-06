import { DatePipe } from '@angular/common';
import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, NgControl, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { map, take } from 'rxjs/operators';
import { Accounto } from 'src/app/_models/Accounto';
import { BetHistoryResulrt } from 'src/app/_models/BetHistoryResulrt';
import { Deposite } from 'src/app/_models/Deposite';
import { DepositeHistoryReult } from 'src/app/_models/DepositeHistoryReult';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { WithDraw } from 'src/app/_models/WithDraw';
import { WithDrawHistory } from 'src/app/_models/WithDrawHistory';
import { authService } from 'src/app/_services/auth.Service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  member: Member;
  user: User;
  AccountInfo: Accounto;
  bsConfigu: Partial<BsDatepickerConfig>;
  pagination: Pagination;
  //
  betPagination: Pagination;
  bethistory: BetHistoryResulrt[];
  pageNumber = 1;
  pageSize = 5;
  SortingStatus;
  //Lost
  //field="Won";
  SearchQuery="All";
  //
  paginationDeposite: Pagination;
  Depositehistory: DepositeHistoryReult[];
  pageNumberDeposite = 1;
  pageSizeDeposite = 5;
  SortingStatusDeposite="Ascending";

  DForm: FormGroup;
  WForm: FormGroup;
  CurrencyId=4;
  validationErrors: string[] = [];
  //
  UForm: FormGroup;
  validationErrorsForUpdatePassword: string[]=[];
  validationErrorsForWithDraw: string[]=[];
  //
  paginationWithDraw: Pagination;
  WithDrawhistory: WithDrawHistory[];
  pageNumberWithDraw = 1;
  pageSizeWithDraw = 5;
  SortingStatusWithDraw="Ascending";

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private auth: authService,
    private fb: FormBuilder,) {
      this.auth.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
      this.bsConfigu = {
        containerClass: 'theme-default',
        dateInputFormat: 'MM/DD/YYYY'
      }
    }

  ngOnInit(): void {
    this.LoadMember();
    this.getaccount();
    //this.getBetHistory();
    this.initilizeForm();
    this.initilizeUpdatePasswordForm();
    this.initilizeWithDraw();
  }
  print(){
    console.log(this.DForm.value);
  }
  initilizeForm() {
    this.DForm = this.fb.group ({
      CurrencyId: ['4',Validators.required],
      Amount: ['',Validators.required],
      cardNumber: ['',Validators.required],
      CVV: ['',Validators.required],
    })
  }
  initilizeUpdatePasswordForm() {
    this.UForm = this.fb.group ({
      oldPassword: ['', [ Validators.required , Validators.minLength(8) , Validators.maxLength(12) ]],
      newPassword: ['', [ Validators.required , Validators.minLength(8) , Validators.maxLength(12) ]],
      confirmPassword: ['', [Validators.required , this.matchValues('newPassword')]],
    })
  }
  initilizeWithDraw() {
    this.WForm = this.fb.group ({
      Amount: ['',Validators.required],
      BankAccountNumber: ['',Validators.required],

    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null : {isMatching: true}
    }
  }
  LoadMember() {
    /* this.route.snapshot.paramMap.get('username') */
     this.memberService.getMember(this.user.username).subscribe(
      (res) => {
        console.log('member');
        console.log(res);
        this.member = res;
      })
  }
  getaccount(){
    var name = this.user.username;
    this.memberService.getAccount(name).subscribe(
      res =>{
        console.log('Account');
        console.log(res);
      },error => {
        console.error(error);
      },() => {
        this.AccountInfo= this.memberService.memberAccount;
      }
    );
  }
   getDepositeHistory(){
     this.memberService.getDepositeHistory(this.user,
      this.pageNumberDeposite,
      this.pageSizeDeposite,
      this.SortingStatusDeposite).subscribe(
      res=> {
        console.log(res);
        this.paginationDeposite = res.pagination;
        this.Depositehistory = res.result;
      }
    )
  }
  getBetHistory(){
    /* console.log(this.startDate)
    console.log(this.endDate)

    var Start : string = this.startDate.toLocaleString();
    var End : string = this.endDate.toLocaleString();
    console.log(Start)
    console.log(End)
    var StartRes = Start.split("/").join("-");
    var EndRes = End.split("/").join("-"); */
    //var StartRes = Start.split(',')[0].split("/").join("-");
    //var EndRes = End.split(',')[0].split("/").join("-");
    //console.log(StartRes)
    //console.log(EndRes)
    console.log(this.SortingStatus);
    this.memberService.getbetHistory(this.user,
      this.pageNumber,
      this.pageSize,
      this.SortingStatus,
      this.SearchQuery).subscribe(
      res=> {
        this.betPagination = res.pagination;
        this.bethistory = res.result;
      }
    )
  }
  getWithDrawHistory(){
    /* console.log(this.startDate)
    console.log(this.endDate)

    var Start : string = this.startDate.toLocaleString();
    var End : string = this.endDate.toLocaleString();
    console.log(Start)
    console.log(End)
    var StartRes = Start.split("/").join("-");
    var EndRes = End.split("/").join("-"); */
    //var StartRes = Start.split(',')[0].split("/").join("-");
    //var EndRes = End.split(',')[0].split("/").join("-");
    //console.log(StartRes)
    //console.log(EndRes)
    console.log(this.SortingStatus);
    this.memberService.getWithdrewHistory(this.user,
      this.pageNumberWithDraw,
      this.pageSizeWithDraw,
      this.SortingStatusWithDraw).subscribe(
      res=> {
        this.paginationWithDraw = res.pagination;
        this.WithDrawhistory = res.result;
      }
    )
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.memberService.getbetHistory(this.user,this.pageNumber,this.pageSize,this.SortingStatus,this.SearchQuery);
    //this.getUsers(this.SearchQuery);
    /* this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers(); */
  }
  Deposite(){
    console.log(this.DForm.value);
    var id = this.getDecodedTokenId(this.user.token);
    var cid = this.DForm.get('CurrencyId').value;
    var ccid : number = +cid;
    var amount = this.DForm.get('Amount').value;
    var camount : number = +amount;
    var x : Deposite = {
      MethodId: 2,
      CurrencyId: ccid,
      UserId: id,
      Amount: camount,
      cardNumber: this.DForm.get('cardNumber').value,
      CVV: this.DForm.get('CVV').value,
    }
    console.log(x);
    this.memberService.Deposite(x).subscribe(
      res => {
        console.log(res);
      },error => {
        console.log(error)
      }
    );
  }
  WithDraw(){
    console.log(this.WForm.value);
    var id = this.getDecodedTokenId(this.user.token);
    var am = this.WForm.get('Amount').value;
    var camount : number = +am;
    var ba = this.WForm.get('BankAccountNumber').value;
    var result : WithDraw = {
      UserId : id,
      Amount: camount,
      BankAccountNo: ba
    }
    this.memberService.WithDraw(result).subscribe(
      res => {
        console.log(res);
      },error => {
        console.log(error)
      }
    );
  }
  cancel(){
    this.DForm.reset();
  }
  cancelUpdatePassword(){
    this.UForm.reset();
  }
  cancelWithDraw(){
    this.WForm.reset();
  }
  getDecodedTokenId(token) {
    var x = JSON.parse(atob(token.split('.')[1])).id;
    return x;
  }
  //update
  updatePassword(){
    var id = this.getDecodedTokenId(this.user.token);
    this.memberService.updatePassword(id,this.UForm.value).subscribe(
      res => {
        console.log(res);
      }
    )

  }
}
//"602146be192d6e255cd3c4b9"
