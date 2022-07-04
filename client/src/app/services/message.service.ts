import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IMessage } from '../models/message';
import { IUser } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.baseUrl + "message/";
  private hubUrl = environment.baseHub;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<IMessage[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private readonly http: HttpClient) { }

  public createHubConnection(user: IUser, otherUserName: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "message?user=" + otherUserName, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error));

    this.hubConnection.on("ReceiveMessageThread", messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        this.messageThreadSource.next([...messages, message])
      })
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  public getMessages(container: string): Observable<IMessage[]> {
    return this.http.get<IMessage[]>(this.baseUrl, {
      params: {
        "name": "name",
        "container": container
      }
    });
  }
  
  public getMessageThread(username: string): Observable<IMessage[]> {
    return this.http.get<IMessage[]>(this.baseUrl + "thread/" + username);
  }

  public async sendMessage(name: string, content: string) {
    return this.hubConnection.invoke('SendMessage', { recipientName: name, content })
      .catch(error => console.log(error));
  }

  public deleteMessage(messageId: number): Observable<any> {
    return this.http.delete(this.baseUrl, {
      params: {
        "messageId": messageId
      }
    })
  }
}
