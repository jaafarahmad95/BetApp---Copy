import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { authService } from 'src/app/_services/auth.Service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class EditProfileComponent implements OnInit {
  //@ViewChild('editForm') editForm:NgForm;
  xForm: FormGroup;
  member: Member;
  user: User;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if(this.xForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private authService: authService,
    private memberService: MembersService,
    private toastr: ToastrService,
    private fb: FormBuilder) {

    this.authService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    //this.loadMember();
    this.initilizeForm();
  }

  initilizeForm() {
    this.xForm = this.fb.group ({
      description: [''],
      lookingFor: [''],
      interests: [''],
      city: [''],
      country: ['']
    })
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => {
     /*  this.member = member; */
    })
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() =>{
      this.toastr.success('Profile Updated Successfully');
      this.xForm.reset(this.member);
    })
  }
}
