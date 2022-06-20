import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { delay, map, take } from 'rxjs/operators';
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
  public sort = this.memberService.displayModel.sortBy;
  public gamesName: string[] = [];

  constructor(public readonly accService: AccountService,
    private readonly steamService: SteamService,
    public readonly memberService: MemberService,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private readonly router: Router) { }

  ngOnInit(): void {
    this.getGamesName();
    this.IsFetchAvailiable();
  }

  public turnSort(id: number): void {
    this.sort.forEach((el, i) => {
      if (i !== id) {
        el.reset();
      }
      else {
        el.reverseVal();
      }
    })

    this.fetchDbGamesWithParams(1);
  }

  public resetInput(): void {
    if (this.memberService.displayParams.search === "") {
      this.memberService.displayModel.search = "";
      return;
    }

    this.memberService.displayParams.search = '';
    this.memberService.displayModel.search = '';
    this.fetchDbGamesWithParams(1);
  }

  public getSortValue(id: number): SortObj {
    return this.sort[id];
  }

  public turnFilter(name: string): void {
    const filters = this.memberService.displayModel.filters;

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

      case "backlog":
        filters.backlog = !filters.backlog;
        break;
    }

    this.fetchDbGamesWithParams(1);
  }

  public getFilterVal(name: string): boolean {
    const filters = this.memberService.displayModel.filters;

    if (name === "notSet") {
      return filters.notSet;
    }

    if (name === "inProgress") {
      return filters.inProgress;
    }

    if (name === "completed") {
      return filters.completed;
    }

    return filters.backlog;
  }

  public getGamesName(): void {
    this.accService.currentUser$.pipe(take(1), map(u => u.steamId)).subscribe(steamId => {
      if (!steamId) {
        return;
      }

      this.memberService.getGamesName().pipe(take(1), map(names => {
        this.gamesName = names;
      })).subscribe();
    });
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

  public fetchDbGamesWithParams(pageNumber: number): void {
    this.switchRoute();
    this.memberService.setDisplayParams();
    this.memberService.getPaginatedUserGames(pageNumber).pipe(take(1)).subscribe(games => {
      this.memberService.userGamesSource.next(games);
    });
  }

  private switchRoute(): void {
    if (this.router.url !== "/profile") {
      return;
    }

    this.router.navigateByUrl("");
  }

  public IsFetchAvailiable(): void {
    this.accService.currentUser$.pipe(take(1), map(user => user.steamId))
      .subscribe(steamId => {
        this.steamService.getGamesCount(steamId).pipe(delay(1500),
          map((steamGamesCount) => {
            console.log(`dbgames: ${this.gamesName.length} | steamGames: ${steamGamesCount}`);
            this.displayFetchButton(this.gamesName.length < steamGamesCount);
          }),
          take(1)).subscribe();
      });
  }

  public displayFetchButton(value: boolean): void {
    this.isFetchNeeded = value;
    this.changeDetectorRef.detectChanges();
  }

  public logOut(): void {
    this.accService.logout();
    this.memberService.userGamesSource.next(undefined);
    this.memberService.pagination.totalItems = 0;
    console.log(this.memberService.pagination);
  }
}
