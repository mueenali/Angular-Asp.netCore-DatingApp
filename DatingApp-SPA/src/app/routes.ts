import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListResolver } from './_resolvers/memberList.resolver';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { AuthGuard } from './_guards/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { MemberDetailResolver } from './_resolvers/memberDetail.resolver';
import { MemberEditResolver } from './_resolvers/memberEdit.resolver';
import { UnsavedChangesGuard } from './_guards/unsaved-changes.guard';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent, resolve: { users: MemberListResolver } },
      { path: 'members/:id', component: MemberDetailsComponent, resolve: { user: MemberDetailResolver } },
      { path: 'member/edit', component: MemberEditComponent, resolve: { user: MemberEditResolver }, canDeactivate: [UnsavedChangesGuard] },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent }
    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
