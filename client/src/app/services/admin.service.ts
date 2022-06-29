import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IGameObj } from '../models/gameObj';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.baseUrl + "admin/";

  constructor(private readonly http: HttpClient) { }

  public getGameInfo(appId: number): Observable<IGameObj> {
    return this.http.get<IGameObj>(this.baseUrl + "getGameInfo", {
      params: {
        "appId": appId
      }
    })
  }

  public updateGameImg(data: IGameObj): Observable<IGameObj> {
    return this.http.put<IGameObj>(this.baseUrl + "updateGameImg", data);
  }
}
