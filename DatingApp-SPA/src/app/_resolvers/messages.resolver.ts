import { AlertifyService } from '../_services/alertify.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {Message} from '../_models/message';
import {AuthService} from '../_services/auth.service';
import {MessageService} from '../_services/message.service';
@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
  pageNumber = 1;
  pageSize = 10;
  params = {messageContainer: 'Unread'};
  route = 'users/' + this.authService.decodedToken.nameid + '/messages';

  constructor(private messageService: MessageService, private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    return this.messageService.getAll(this.route, this.pageNumber, this.pageSize, this.params).pipe(
      catchError(error => {
        this.alertify.error('Problem retriving messages');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
