import { UnsavedChangesGuard } from './_guards/unsaved-changes.guard';
import { MemberEditResolver } from './_resolvers/memberEdit.resolver';
import { MemberDetailResolver } from './_resolvers/memberDetail.resolver';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { AuthGuard } from './_guards/auth.guard';
import { AlertifyService } from './_services/alertify.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { BsDropdownModule, TabsModule } from 'ngx-bootstrap';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { UserService } from './_services/user.service';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { MemberListResolver } from './_resolvers/memberList.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PhotoManagementComponent } from './members/photo-management/photo-management.component';
import {FileUploadModule} from 'ng2-file-upload';
import {PhotoService} from './_services/photo.service';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MessagesComponent,
    ListsComponent,
    MemberCardComponent,
    MemberDetailsComponent,
    MemberEditComponent,
    PhotoManagementComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    TabsModule.forRoot(),
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    NgxGalleryModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ['localhost:5000'],
        blacklistedRoutes: ['localhost:5000/api/auth']
      }
    }),
    FileUploadModule
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyService,
    AuthGuard,
    UnsavedChangesGuard,
    UserService,
    PhotoService,
    MemberDetailResolver,
    MemberEditResolver,
    MemberListResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
