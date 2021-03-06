import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ISteamGame } from '../models/steamGame';
import { ISteamUser } from '../models/steamUser';

@Injectable({
  providedIn: 'root'
})
export class SteamService {

  public steamGamesSource = new ReplaySubject<ISteamGame[]>(1);
  public steamGames$ = this.steamGamesSource.asObservable();

  private steamBaseUrl = environment.baseSteam;

  constructor(private readonly http: HttpClient) { }

  public getUserSteamGames(id: string): Observable<ISteamGame[]> {
    return this.http.get(this.steamBaseUrl + "IPlayerService/GetOwnedGames/v0001/", {
      params: {
        "key": environment.steamApiKey,
        "steamid": id,
        "format": "json",
        "include_played_free_games": true,
        "include_appinfo": true
      }
    }).pipe(map((r: any) => {
      const games: ISteamGame[] = [];
      r?.response?.games?.map((game: ISteamGame) => {

        game.img_icon_url = game.img_icon_url ?
          `http://media.steampowered.com/steamcommunity/public/images/apps/${game.appid}/${game.img_icon_url}.jpg`
            : "./assets/unknownIcon.png";

        game.playtime_forever = Math.round((game.playtime_forever / 60) * 100) / 100;
        games.push(game);
      });
      return games;
    }));
  }

  public getGamesCount(id: string): Observable<number> {
    return this.http.get(this.steamBaseUrl + "IPlayerService/GetOwnedGames/v0001/", {
      params: {
        "key": environment.steamApiKey,
        "steamid": id,
        "format": "json",
        "include_played_free_games": true,
      }
    }).pipe(
      map((r: any) => {
        return r?.response?.game_count;
      })
    );
  }

  public getMemberProfileInfo(steamId: string): Observable<ISteamUser> {
    return this.http.get<ISteamUser>(this.steamBaseUrl + "ISteamUser/GetPlayerSummaries/v0002/", {
      params: {
        "key": environment.steamApiKey,
        "steamids": steamId,
        "format": "json"
      }
    }).pipe(map((r: any) => {
      const playerData: any[] = r?.response?.players;
      return playerData[0];
    }))
  }

  public getGameImg(appId: number): Observable<string> {
    return this.http.get("https://store.steampowered.com/api/appdetails", {
      params: {
        "appids": appId.toString(),
        "key": ""
      }
    }).pipe(map((response: any) => {
      if (!response[`${appId}`]?.success) {
        const err: any = new Error();
        err["data"] = `Steam error: No data found for app: ${appId}`;
        throw err;
      }

      return response[`${appId}`]?.data?.header_image;
    }));
  }
}
