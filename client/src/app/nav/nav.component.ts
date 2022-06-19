import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';
import { SteamService } from '../services/steam.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavComponent implements OnInit {

  constructor(public readonly accService: AccountService, private readonly steamService: SteamService) { }

  public fetchSteamGames(steamId: string): void {
    this.steamService.getUserSteamGames(steamId).pipe(take(1)).subscribe(games => games);
  }
  
  ngOnInit(): void {
  }

}
