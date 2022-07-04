import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { IGameObj } from '../models/gameObj';
import { ISteamGame } from '../models/steamGame';
import { IUser } from '../models/user';
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

  constructor(public readonly accService: AccountService,
    public readonly steamService: SteamService,
    public readonly memberService: MemberService,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private readonly toastr: ToastrService) { }

  ngOnInit(): void {
    this.onUserLogin();
  }

  public getGamesFromBd(): void {
    const pag = this.memberService.pagination;

    if (pag.currentPage >= pag.totalPages) {
      return;
    }

    forkJoin([this.memberService.getPaginatedUserGames(pag.currentPage + 1)
      .pipe(take(1)), this.memberService.games$.pipe(take(1))]).pipe(
        take(1),
        map(([newGames, currentGames]) => {
          newGames.forEach(game => {
            currentGames.push(game);
          });
          this.memberService.userGamesSource.next(currentGames);
        })
      ).subscribe();

    this.changeDetectorRef.markForCheck()
  }

  private onUserLogin(): void {
    this.accService.currentUser$.pipe(take(1)).subscribe((user: IUser) => {
      this.memberService.getPaginatedUserGames(1).pipe(take(1)).subscribe(games => {
        this.memberService.userGamesSource.next(games);
      })

      if (user.steamId) {
        this.fetchSteamGames();
      }
    });
  }

  private fetchSteamGames(): void {
    forkJoin([this.memberService.games$.pipe(take(1)), this.memberService.getUserDbGames()])
      .pipe(map(([pagedGames, dbGames]) => {
        this.steamService.steamGames$.pipe(take(1)).subscribe((steamGames: ISteamGame[]) => {
          if (steamGames.length === 0) {
            return;
          }
          
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

          this.memberService.addGames(gamesToAdd).pipe(take(1)).subscribe((newGames) => {
            this.toastr.success(`fetched ${newGames.length} games.`);
            this.memberService.memberCache.clear();
            this.memberService.getPaginatedUserGames(1).pipe(take(1))
              .subscribe((games: IGameObj[]) => {
                this.memberService.userGamesSource.next(games);
                this.changeDetectorRef.markForCheck();
                this.steamService.steamGamesSource.next([]);
              })
          });
        })
      })).pipe(take(1)).subscribe(() => { }, err => {
        this.toastr.error("error while fetching games");
      });
  }

  public loadButtonDisplay(): boolean {
    const pag = this.memberService.pagination;
    return pag.currentPage < pag.totalPages
      && pag.totalItems > 0;
  }
}