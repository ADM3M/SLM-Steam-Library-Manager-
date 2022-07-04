import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { IMessage } from '../models/message';
import { IUser } from '../models/user';
import { AccountService } from '../services/account.service';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatComponent implements OnInit, OnDestroy {
  public messages: IMessage[] = [];
  public messageContent = "";
  private username: string;
  private user: IUser;


  constructor(public readonly messageService: MessageService,
    private readonly route: ActivatedRoute,
    private readonly changeDetector: ChangeDetectorRef,
    private accService: AccountService) {
      this.accService.currentUser$.pipe(take(1)).subscribe((user: IUser) => {
        this.user = user;
      });
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    this.resolveUserFromRoute();
    this.messageService.createHubConnection(this.user, this.username);
  }

  private resolveUserFromRoute(): void {
    this.route.queryParams.pipe(take(1))
      .subscribe(params => {
        this.username = params?.username;

        this.messageService.getMessageThread(this.username).pipe(take(1))
          .subscribe((messages: IMessage[]) => {
            this.messages = messages;
            this.changeDetector.markForCheck();
          })
      });
  }

  public sendMessage(form: NgForm): void {
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      form.reset();
    })
  }
}
