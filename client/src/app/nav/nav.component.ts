import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { delay, filter, map, take } from 'rxjs/operators';
import { IGameObj } from '../models/gameObj';
import { SortObj } from '../models/sortObj';
import { AccountService } from '../services/account.service';
import { MemberService } from '../services/member.service';
import { SteamService } from '../services/steam.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class NavComponent implements OnInit {
  
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
    this.turnFilter("reset");
    this.memberService.setDisplayParams();
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

      case "enable_all":
        filters.notSet = true;
        filters.inProgress = true;
        filters.completed = true;
        filters.backlog = true;
        return;
      
      case "reset":
        filters.notSet = false;
        filters.inProgress = true;
        filters.completed = false;
        filters.backlog = false;
        return;
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
    if (!this.memberService.isFetchNeeded) {
      return;
    }

    this.steamService.getUserSteamGames(steamId)
      .pipe(take(1))
      .subscribe(games => {
        this.memberService.userGamesSource.next([]);
        this.switchRoute();
        this.steamService.steamGamesSource.next(games);
        this.displayFetchButton(false);
        of(true).pipe(delay(500)).subscribe(() => {
          this.getGamesName();
        })
      })
  }

  public fetchDbGamesWithParams(pageNumber: number): void {
    this.switchRoute();
    this.memberService.setDisplayParams();
    this.memberService.getPaginatedUserGames(pageNumber).pipe(take(1)).subscribe(games => {
      this.memberService.userGamesSource.next(games);
    });
  }

  public search(): void {
    this.switchRoute();
    this.turnFilter("enable_all");
    this.memberService.setDisplayParams();
    this.memberService.getPaginatedUserGames(1).pipe(take(1)).subscribe(games => {
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
        
        if (!steamId) return;

        this.steamService.getGamesCount(steamId).pipe(delay(1500),
          map((steamGamesCount) => {
            console.log(`dbgames: ${this.gamesName.length} | steamGames: ${steamGamesCount}`);
            this.displayFetchButton(this.gamesName.length < steamGamesCount);
          }),
          take(1)).subscribe();
      });
  }

  public displayFetchButton(value: boolean): void {
    this.memberService.isFetchNeeded = value;
    this.changeDetectorRef.detectChanges();
  }

  public logOut(): void {
    this.accService.logout();
    this.memberService.userGamesSource.next(undefined);
    this.memberService.pagination.totalItems = 0;
    this.memberService.memberCache.clear();
    this.router.navigateByUrl("/crutch");
  }

  public onTypeaheadSelect(event: any) {
    this.search();
  }
}
