import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { ISteamGame } from '../models/steamGame';
import { AccountService } from '../services/account.service';
import { MemberService } from '../services/member.service';
import { SteamService } from '../services/steam.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {

  constructor(public readonly accService: AccountService, public readonly steamService: SteamService, public readonly memberService: MemberService) { }

  private getGamesFromBd(): void {
    this.memberService.getUserDbGames().pipe(take(1)).subscribe();
  }

  private onUserLogin(): void {
    this.accService.currentUser$.pipe(take(1)).subscribe(() => {
      this.getGamesFromBd();
      this.fetchSteamGames();
    });
  }

  private fetchSteamGames(): void {
    this.memberService.games.pipe(take(1)).subscribe(dbGames => {
      this.steamService.steamGames.pipe(take(1)).subscribe((steamGames: ISteamGame[]) => {
        let gamesToAdd: ISteamGame[] = [];

        if (dbGames.length === 0) {
          gamesToAdd = steamGames;
        }
        else if (dbGames.length !== steamGames.length) {

          steamGames.forEach(sg => {
            let isNew = true;
            dbGames.forEach(dg => {
              if (dg.appId === sg.appid) {
                isNew = false;
                return;
              }
            });

            if (isNew) gamesToAdd.push(sg);
          });

        }
        else {
          // TODO: print toast
          console.log('all games are up to date');
          return;
        }

        this.memberService.addGames(gamesToAdd).pipe(take(1)).subscribe((newGames) => {
          this.memberService.userGamesSource.next(dbGames.concat(newGames));
        });
      })
    })
  }

  ngOnInit(): void {
    this.onUserLogin();
  }

}
