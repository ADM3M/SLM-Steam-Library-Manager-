import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IGameObj } from '../models/gameObj';
import { ISteamGame } from '../models/steamGame';
import { ISteamUser } from '../models/steamUser';
import { IUser } from '../models/user';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.baseUrl;
  public userGamesSource = new ReplaySubject<IGameObj[]>(1);
  public games = this.userGamesSource.asObservable();

  constructor(private readonly http: HttpClient, private readonly accService: AccountService) { }

  public getUserDbGames(): Observable<IGameObj[]> {
    return this.http.get<IGameObj[]>(this.baseUrl + "user").pipe(
      map(games => {
        this.userGamesSource.next(games);
        return games;
      })
    )
  }

  public updateGame(gameData: IGameObj): Observable<IGameObj> {
    return this.http.put<IGameObj>(this.baseUrl + "games/update", gameData)
  }

  public updateSteamUserData(steamData: ISteamUser): Observable<IUser> {
    return this.http.put<IUser>(this.baseUrl + "user/updateSteamId", null, {
      params: {
        "steamId": steamData.steamid,
        "photoUrl": steamData.avatarmedium
      }
    });
  }

  public addGames(steamGames: ISteamGame[]): Observable<IGameObj[]> {
    return this.http.post<IGameObj[]>(this.baseUrl + "user/addgames", steamGames);
  }

  public updateGameStatus(gameData: IGameObj): Observable<IGameObj> {
    return this.http.put<IGameObj>(this.baseUrl + "user/updateGameStatus", gameData);
  }
}
