import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnInit, TemplateRef } from '@angular/core';
import { forkJoin, Observable, of, Subject, zip } from 'rxjs';
import { delay, map, take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';
import { MemberService } from '../services/member.service';
import { SteamService } from '../services/steam.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavComponent implements OnInit {
  public isFetchNeeded = false;

  constructor(public readonly accService: AccountService,
    private readonly steamService: SteamService,
    public readonly memberService: MemberService,
    private readonly changeDetectorRef: ChangeDetectorRef) { }


  public fetchSteamGames(steamId: string): void {

    if (!this.isFetchNeeded) {
      return;
    }

    this.steamService.getUserSteamGames(steamId)
      .pipe(take(1))
      .subscribe(games => {
        this.steamService.steamGamesSource.next(games);
        this.displayFetchButton(false);
      })
  }

  public getDbGamesName(): Observable<string[]> {
    return this.memberService.getGamesName()
  }

  public IsFetchAvailiable(): void {
    this.accService.currentUser$.pipe(take(1), map(user => user.steamId))
      .subscribe(steamId => {
        forkJoin([this.getDbGamesName(), this.steamService.getGamesCount(steamId)]).pipe(
          delay(1500),
          map(([dbGameNames, steamGamesCount]) => {
            console.log(`dbgames: ${dbGameNames.length} | steamGames: ${steamGamesCount}`);
            this.displayFetchButton(dbGameNames.length < steamGamesCount);
          }),
          take(1)).subscribe();
      })
  }

  public displayFetchButton(value: boolean): void {
    this.isFetchNeeded = value;
    this.changeDetectorRef.detectChanges();
  }

  public logOut() {
    this.accService.logout();
  }

  ngOnInit(): void {
    this.IsFetchAvailiable();
  }

}
