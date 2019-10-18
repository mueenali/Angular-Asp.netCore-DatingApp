import { Injectable } from '@angular/core';
import {BaseService} from './base.service';
import {Message} from '../_models/message';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MessageService extends BaseService<Message> {

  constructor(http: HttpClient) {
    super(http);
  }

  getMessageThread(senderId: number, receiverId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + senderId + '/messages/thread/' + receiverId);
  }

  markAsRead(userId: number, messageId: number) {
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }

}

