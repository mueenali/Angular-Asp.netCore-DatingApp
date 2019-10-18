import { Component, OnInit } from '@angular/core';
import {Message} from '../_models/message';
import {PaginatedResult, Pagination} from '../_models/pagination';
import {AuthService} from '../_services/auth.service';
import {ActivatedRoute} from '@angular/router';
import {AlertifyService} from '../_services/alertify.service';
import {MessageService} from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Set<Message>;
  pagination: Pagination;
  messageContainer = 'Unread';
  params: any = {};

  constructor(private messageService: MessageService, private authService: AuthService,
              private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = new Set<Message>(data['messages'].result);
      this.pagination = data['messages'].pagination;
      console.log(this.messages);
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  loadMessages() {
    const route = 'users/' + this.authService.decodedToken.nameid + '/messages';
    this.params.messageContainer = this.messageContainer;
    this.messageService.getAll(route, this.pagination.currentPage, this.pagination.itemsPerPage, this.params).
    subscribe((res: PaginatedResult<Message[]> ) => {
      this.messages = new Set<Message>(res.result);
      this.pagination = res.pagination;
    }, error => this.alertify.error(error));
  }

  deleteMessage(message: Message) {
    this.alertify.confirm('Are you sure you want to delete this message?', () => {
      const route = 'users/' + this.authService.decodedToken.nameid + '/messages';
      this.messageService.delete(route, message.id).subscribe(() => {
        this.messages.delete(message);
        this.alertify.success('Message deleted successfully');
      }, error => this.alertify.error(error));
    });
  }

}
