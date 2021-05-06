
import { Directive,  Input,  OnInit,  TemplateRef,  ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { authService } from '../_services/auth.Service';

@Directive({
  selector: '[appHasRole]' //*appHasRole='["Admin"]'
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[];
  user: User;
  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private auth: authService,) {
      this.user = this.auth.getCurrentUser();
    }

  ngOnInit(): void {
    //Clear the view if no roles
    if (!this.user?.roles || this.user == null){
      this.viewContainerRef.clear();
      return;
    }
    if(this.user?.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }else {
      this.viewContainerRef.clear();
    }
  }

}
