import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnInit, TemplateRef } from '@angular/core';
import { forkJoin, Observable, of, Subject, zip } from 'rxjs';
import { delay, map, take } from 'rxjs/operators';
import { DisplayParams } from '../models/displayParams';
import { SortObj } from '../models/sortObj';
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
  public sort = this.memberService.sortBy;
  public displayParams = new DisplayParams();

  constructor(public readonly accService: AccountService,
    private readonly steamService: SteamService,
    public readonly memberService: MemberService,
    private readonly changeDetectorRef: ChangeDetectorRef) { }

  turnSort(id: number): void {
    this.sort.forEach((el, i) => {
      if (i !== id) {
        el.reset();
      }
      else {
        el.reverseVal();
      }
    })
  }

  getSortValue(id: number): SortObj {
    return this.sort[id];
  }

  public turnFilter(name: string): void {
    const filters = this.memberService.filters;

    switch (name) {
      case "notSet":
        filters.notSet = !filters.notSet;
        break;

      case "inProgress":
        filters.inProgress = !filters.inProgress;
        break;

      case "completed":
        filters.completed = !filters.completed;
        break;
    }
  }

  public getFilterVal(name: string): boolean {
    if (name === "notSet") {
      return this.memberService.filters.notSet;
    }

    if (name === "inProgress") {
      return this.memberService.filters.inProgress;
    }

    return this.memberService.filters.completed;
  }

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
    this.memberService.getGamesName().pipe(take(1)).subscribe();
  }

}
