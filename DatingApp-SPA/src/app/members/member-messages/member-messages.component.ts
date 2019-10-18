import {Component, Input, OnInit} from '@angular/core';
import {MessageService} from '../../_services/message.service';
import {Message} from '../../_models/message';
import {AuthService} from '../../_services/auth.service';
import {AlertifyService} from '../../_services/alertify.service';
import {User} from '../../_models/user';
import {tap} from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() receiver: User;
  messages: Set<Message>;
  newMessage: any = {};
  constructor(private messageService: MessageService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessageThread();
  }

  loadMessageThread() {
    const currentUserId = this.authService.decodedToken.nameid;
    this.messageService.getMessageThread(this.authService.decodedToken.nameid, this.receiver.id)
      .pipe(
        tap(messages => {
          for (let i = 0; i < messages.length; i++) {
            if (messages[i].isRead === false && messages[i].receiverId === currentUserId) {
              this.messageService.markAsRead(currentUserId, messages[i].id);
            }
          }
        })
      )
      .subscribe(messages => {
      this.messages = new Set<Message>(messages);
    }, error => this.alertify.error(error));
  }

  sendMessage() {
    this.newMessage.receiverId = this.receiver.id;
    const route = 'users/' + this.authService.decodedToken.nameid + '/messages';
    this.messageService.add(route, this.newMessage).subscribe((message: Message) => {
       this.messages.add(message);
       this.newMessage.content = '';
    }, error => this.alertify.error(error));
  }
}
