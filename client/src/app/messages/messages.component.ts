import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { IMessage } from '../models/message';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MessagesComponent implements OnInit {
  public messages: IMessage[] = [];
  public container = "Inbox";
  public loading = false;

  constructor(private readonly messageService: MessageService,
    private readonly changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  public loadMessages(): void {
    this.messageService.getMessages(this.container).pipe(take(1))
      .subscribe((messages: IMessage[]) => {
        this.messages = messages;
        this.changeDetector.markForCheck();
      });
  }

}
