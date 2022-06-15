import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
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

  public fetchSteamGames(steamId: string) {
    this.steamService.getUserSteamGames(steamId).subscribe(games => games);
  }
  
  ngOnInit(): void {
  }

}
