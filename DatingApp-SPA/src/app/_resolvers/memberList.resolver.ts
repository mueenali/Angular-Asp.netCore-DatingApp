import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 6;
  params: any = {};
  constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {
    const user = JSON.parse(localStorage.getItem('user'));
    user.gender === 'male' ? this.params.gender = 'female' : this.params.gender = 'male';
  }
  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService.getAll('users', this.pageNumber, this.pageSize, this.params).pipe(
      catchError(error => {
        this.alertify.error('Problem retriving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
