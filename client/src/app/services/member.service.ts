import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DisplayParams } from '../models/displayParams';
import { SortObj } from '../models/sortObj';
import { IGameObj } from '../models/gameObj';
import { ISteamGame } from '../models/steamGame';
import { ISteamUser } from '../models/steamUser';
import { IUser } from '../models/user';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';
import { IPagination, PaginatedResult } from '../models/pagination';
import { GameState } from '../enums/gameState';
import { IUserSummary } from '../models/userSummary';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  public userGamesSource = new ReplaySubject<IGameObj[]>(1);
  public pagination: IPagination = { currentPage: 0, itemsPerPage: 0, totalItems: 0, totalPages: 0 }
  public games$ = this.userGamesSource.asObservable();
  public isFetchNeeded = false;
  public memberCache = new Map<string, Observable<IGameObj[]>>();
  private baseUrl = environment.baseUrl;

  public displayModel = {
    filters: {
      notSet: false,
      inProgress: true,
      completed: false,
      backlog: false
    },

    sortBy: [
      new SortObj("name", false, false),
      new SortObj("status", false, false),
      new SortObj("timePlayed", true, true)
    ],

    search: ""
  }

  public displayParams = new DisplayParams();

  constructor(private readonly http: HttpClient) { }

  public setDisplayParams(): void {
    const sortObj = this.displayModel.sortBy.find(v => v.value);
    this.displayParams.orderBy = sortObj!.name.concat(sortObj!.reverse ? "Reverse" : "");

    const filterObj = this.displayModel.filters;
    this.displayParams.statusesToShow = "";

    if (filterObj.notSet) {
      this.displayParams.statusesToShow += "0";
    }

    if (filterObj.inProgress) {
      this.displayParams.statusesToShow += "1";
    }

    if (filterObj.completed) {
      this.displayParams.statusesToShow += "2";
    }

    if (filterObj.backlog) {
      this.displayParams.statusesToShow += "3";
    }

    this.displayParams.search = this.displayModel.search;
  }

  public getPaginatedUserGames(pageNumber: number): Observable<IGameObj[]> {
    const dp = this.displayParams;
    this.pagination.currentPage = pageNumber;
    const response = this.memberCache.get(this.displayParams.join() + "-" + Object.values(this.pagination).join("-"));

    if (response) {
      return response;
    }

    let httpParams = getPaginationHeader(pageNumber)
      .append("orderBy", dp.orderBy)
      .append("statusesToShow", dp.statusesToShow)
      .append("search", dp.search);

    return getPaginatedResult<IGameObj[]>(this.baseUrl + "user", this.http, httpParams)
      .pipe(map((r: PaginatedResult<IGameObj[]>) => {
        this.pagination = r.pagination;
        this.memberCache.set(this.displayParams.join() + "-" + Object.values(this.pagination).join("-"), of(r.result!));
        return r.result!;
      }))
  }

  public getUserDbGames(): Observable<IGameObj[]> {
    return this.http.get<IGameObj[]>(this.baseUrl + "user", { params: { "pageNumber": "-1" } }).pipe(
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
        "photoUrl": steamData.avatarmedium ?? "./assets/unknownImg.jpg"
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
    return this.http.get<string[]>(this.baseUrl + "user/getGamesName").pipe(map((names: string[]) => {
      return names;
    }))
  }

  public getUserSummaries(): Observable<IUserSummary> {
    return this.getUserDbGames().pipe(take(1), map(games => {
      const userSum = <IUserSummary>{ completed: 0, inProgress: 0, notSet: 0, backlog: 0, total: 0 }
      userSum.total = games.length;
      games.forEach(game => {
        switch (game.status) {
          case GameState.Completed:
            userSum.completed++;
            break;

          case GameState.InProgress:
            userSum.inProgress++;
            break;

          case GameState.NotSet:
            userSum.notSet++;
            break;

          case GameState.Backlog:
            userSum.backlog++;
        }
      });

      const lastItems = [];
      for (let i = games.length - 1; i >= games.length - 10; i--) {
        lastItems.push(games[i]);
      }

      userSum.lastGames = lastItems;
      return userSum;
    }));
  }

  public getUserIdByUrl(url: string): Observable<string> {
    return this.http.get("http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/", {
      params: {
        key: environment.steamApiKey,
        vanityurl: url
      }
    }).pipe(take(1),
      map((response: any) => {
        if (response?.response?.success == "1") {
          return response?.response?.steamid;
        }

        // TODO: toast
        throw new Error("profile not found");
      }))
  }
}
