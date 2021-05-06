import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxSpinnerModule } from "ngx-spinner";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { FooterComponent } from './footer/footer.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { PopLoginComponent } from './pop-login/pop-login.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { BackTopComponent } from './back-top/back-top.component';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { ProfileComponent } from './members/profile/profile.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
//import {GoTopButtonModule} from 'ng-go-top-button';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { EditProfileComponent } from './members/edit-profile/edit-profile.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccordionModule } from 'ngx-bootstrap/accordion';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { NgScrollbarModule } from 'ngx-scrollbar';
import { LiveComponent } from './live/live.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { LoginComponent } from './login/login.component';
import {DatePipe} from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    RegisterComponent,
    HomeComponent,
    FooterComponent,
    TextInputComponent,
    DateInputComponent,
    PopLoginComponent,
    NotFoundComponent,
    ServerErrorComponent,
    BackTopComponent,
    ProfileComponent,
    EditProfileComponent,
    LiveComponent,
    AdminPanelComponent,
    HasRoleDirective,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    BsDropdownModule.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    BsDatepickerModule.forRoot(),
    ModalModule.forRoot(),
    NgxSpinnerModule,
    TabsModule.forRoot(),
    PaginationModule.forRoot(),
    AccordionModule.forRoot(),
    CollapseModule.forRoot(),
    NgScrollbarModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS , useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS , useClass: LoadingInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS , useClass: JwtInterceptor, multi: true},
    DatePipe,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule { }
