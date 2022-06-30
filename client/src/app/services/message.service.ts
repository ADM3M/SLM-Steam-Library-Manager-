import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IMessage } from '../models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.baseUrl + "message/";

  constructor(private readonly http: HttpClient) { }

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

  public sendMessage(username: string, content: string): Observable<IMessage> {
      return this.http.post<IMessage>(this.baseUrl, {
        "recipientName": username,
        content
      });
  }

  public deleteMessage(messageId: number): Observable<any> {
    return this.http.delete(this.baseUrl, {
      params: {
        "messageId": messageId
      }
    })
  }
}
