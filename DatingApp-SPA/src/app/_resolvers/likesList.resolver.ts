import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
@Injectable()
export class LikesListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 10;
  likesParams = {likers: true};
  constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService.getAll('users', this.pageNumber, this.pageSize, this.likesParams).pipe(
      catchError(error => {
        this.alertify.error('Problem retriving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
