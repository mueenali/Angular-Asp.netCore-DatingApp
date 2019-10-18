import { Component, OnInit } from '@angular/core';
import {User} from '../_models/user';
import {PaginatedResult, Pagination} from '../_models/pagination';
import {AuthService} from '../_services/auth.service';
import {UserService} from '../_services/user.service';
import {ActivatedRoute} from '@angular/router';
import {AlertifyService} from '../_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  params: any = {};
  likesParam: string;
  constructor(private authService: AuthService, private userService: UserService, private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
      console.log(this.users);
    });
    this.params.likers = true;
  }

  loadUsers() {
    if (this.likesParam === 'likers') {
      this.params.likers = true;
    } else {
      this.params.likees = true;
      this.params.likers = false;
    }

    this.userService.getAll('users', this.pagination.currentPage, this.pagination.itemsPerPage, this.params).
    subscribe((response: PaginatedResult<User[]>) => {
      this.users = response.result;
      this.pagination = response.pagination;
    }, error => this.alertify.error(error));
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
}
