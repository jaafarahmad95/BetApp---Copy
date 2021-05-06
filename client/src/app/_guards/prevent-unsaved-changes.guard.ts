import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginComponent } from '../login/login.component';
import { RegisterComponent } from '../register/register.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(component: RegisterComponent): boolean {
    if (component.xForm.dirty)
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost!!!');
    return true;
  }

}
