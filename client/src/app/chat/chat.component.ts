import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { IMessage } from '../models/message';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  public messages: IMessage[] = [];
  public messageContent = "";
  private username: string;


  constructor(private readonly messageService: MessageService,
    private readonly route: ActivatedRoute,
    private readonly changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
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
    this.messageService.sendMessage(this.username, this.messageContent)
      .pipe(take(1)).subscribe((message: IMessage) => {
        this.messages.push(message);
        this.changeDetector.markForCheck();
        form.reset();
      })
  }
}
