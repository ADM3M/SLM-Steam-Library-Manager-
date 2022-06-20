import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { map, take } from 'rxjs/operators';
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

  constructor(public readonly accService: AccountService,
    public readonly steamService: SteamService,
    public readonly memberService: MemberService,
    private readonly changeDetectorRef: ChangeDetectorRef) { }

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
    this.accService.currentUser$.pipe(take(1)).subscribe(() => {
      this.memberService.getPaginatedUserGames(1).pipe(take(1)).subscribe(games => {
        this.memberService.userGamesSource.next(games);
      })

      this.fetchSteamGames();
    });
  }

  private fetchSteamGames(): void {
    forkJoin([this.memberService.games$.pipe(take(1)), this.memberService.getUserDbGames()])
      .pipe(map(([pagedGames, dbGames]) => {
        this.steamService.steamGames$.pipe(take(1)).subscribe((steamGames: ISteamGame[]) => {
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
            this.memberService.userGamesSource.next(newGames.concat(pagedGames));
          });
        })
      })).pipe(take(1)).subscribe();
  }

  public loadButtonDisplay(): boolean {
    const pag = this.memberService.pagination;
    return pag.currentPage < pag.totalPages
      && pag.totalItems > 0;
  }

  // infinite scroll

  // trackScroll() {
  //   fromEvent(window, "scroll").subscribe(() => {
  //     if (document.documentElement.scrollHeight - window.scrollY - window.innerHeight < 0.0003 ) {
  //       console.log(this.memberService.pagination);
  //       if (this.memberService.pagination.currentPage < this.memberService.pagination.totalPages) {
  //         this.getGamesFromBd(this.memberService.pagination.currentPage + 1);
  //       }
  //     }
  //   })
  // }
}