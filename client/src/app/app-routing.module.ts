import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { HomeComponent } from './home/home.component';
import { LiveComponent } from './live/live.component';
import { LoginComponent } from './login/login.component';
import { ProfileComponent } from './members/profile/profile.component';
import { RegisterComponent } from './register/register.component';
import { AdminGuard } from './_guards/admin.guard';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';

const routes: Routes = [
  //{path: '',component: HomeComponent},
  {path: '', component: LoginComponent},
  {path: 'home',component: HomeComponent},
  {path: ':region/:competition',component:HomeComponent},
  {path: 'live',component: LiveComponent},
  {path: 'register',component: RegisterComponent,canDeactivate: [PreventUnsavedChangesGuard]},
  {path: 'login',component: LoginComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
        {path: 'admin',component: AdminPanelComponent, canActivate:[AdminGuard]},
        {path: 'members/profiles/:username',component: ProfileComponent, pathMatch: 'full'},
        //{path: 'members/profile/edit',component:EditProfileComponent, canDeactivate: [PreventUnsavedChangesGuard]},
    ]
  },

  {path: 'not-found',component: NotFoundComponent},
  {path:'server-error',component: ServerErrorComponent},
  {path: '**',component: NotFoundComponent, pathMatch: 'full'},
];
/* const routes: Routes = [
  {path: '',component: HomeComponent},
  //{path: '', component: LoginComponent},
  //{path: 'home',component: HomeComponent},
  {path: ':region/:competition',component:HomeComponent},
  {path: 'live',component: LiveComponent},
  {path: 'register',component: RegisterComponent,canDeactivate: [PreventUnsavedChangesGuard]},
  {path: 'login',component: LoginComponent},
  {
    path:'',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
        {path: 'admin',component: AdminPanelComponent, canActivate:[AdminGuard]},
        {path: 'members/profiles/:username',component: ProfileComponent, pathMatch: 'full'},
        //{path: 'members/profile/edit',component:EditProfileComponent, canDeactivate: [PreventUnsavedChangesGuard]},
    ]
  },

  {path: 'not-found',component: NotFoundComponent},
  {path:'server-error',component: ServerErrorComponent},
  {path: '**',component: NotFoundComponent, pathMatch: 'full'},
]; */

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

