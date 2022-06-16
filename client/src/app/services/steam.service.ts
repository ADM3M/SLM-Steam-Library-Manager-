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

  private steamGamesSource = new ReplaySubject<ISteamGame[]>(1);
  public steamGames = this.steamGamesSource.asObservable();
  
  private steamBaseUrl = environment.baseSteam;

  constructor(private readonly http: HttpClient) { }

  public getUserSteamGames(id: string): Observable<ISteamGame[]> {
    return this.http.get(this.steamBaseUrl + "IPlayerService/GetOwnedGames/v0001/", {
      params: {
        "key": environment.steamApiKey,
        "steamid" : id,
        "format": "json",
        "include_played_free_games": true,
        "include_appinfo": true
      }
    }).pipe(map((r: any) => {
      const games: ISteamGame[] = [];
      r?.response?.games?.map((game: ISteamGame) => {
        game.img_icon_url = `http://media.steampowered.com/steamcommunity/public/images/apps/${game.appid}/${game.img_icon_url}.jpg`;
        game.playtime_forever = Math.round((game.playtime_forever / 60) * 100) / 100;
        games.push(game);
      });

      this.steamGamesSource.next(games);
      return games;
    }));
  }

  public getMemberProfileInfo(steamId: string): Observable<ISteamUser> {
    return this.http.get<ISteamUser>(this.steamBaseUrl + "ISteamUser/GetPlayerSummaries/v0002/", {params: {
      "key": environment.steamApiKey,
      "steamids": steamId,
      "format": "json"
    }}).pipe(map((r: any) => {
      const playerData: any[] = r?.response?.players;
      return playerData[0];
    }))
  }

}
