import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DisplayParams } from '../models/displayParams';
import { SortObj } from '../models/sortObj';
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
  public gamesName: Observable<string[]> = new Observable();
  public names: string[] = [];
  public filters = {
    notSet: true,
    inProgress: true,
    completed: true
  }

  public sortBy: SortObj[] = [
      new SortObj("name", true, true),
      new SortObj("status", false, false),
      new SortObj("time", false, false)
  ]

  public search = "";

  public displayParams = new DisplayParams();

  constructor(private readonly http: HttpClient, private readonly accService: AccountService) { }

  public getUserDbGames(pageNumber: number = -1): Observable<IGameObj[]> {
    return this.http.get<IGameObj[]>(this.baseUrl + "user", {params: {"pageNumber":  pageNumber}}).pipe(
      map(games => {
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

  public getGamesName(): Observable<string[]> {
    if (this.names && this.names.length) {
      return this.gamesName;
    }

    return this.http.get<string[]>(this.baseUrl + "user/getGamesName").pipe(map((names: string[]) => {
      this.names = names;
      this.gamesName = of(names);
      return names;
    }))
  }
}
