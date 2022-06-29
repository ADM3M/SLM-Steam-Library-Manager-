import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { take } from 'rxjs/operators';
import { IGameObj } from 'src/app/models/gameObj';
import { AdminService } from 'src/app/services/admin.service';
import { SteamService } from 'src/app/services/steam.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminComponent implements OnInit {
  public appId = "";
  public oldGameEnt: IGameObj;
  public newGameImg = "";
  
  constructor(private readonly adminService: AdminService,
    private readonly steamService: SteamService,
    private readonly changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  public GetGameInfo(): void {
    forkJoin([this.adminService.getGameInfo(Number(this.appId)).pipe(take(1)),
    this.steamService.getGameImg(Number(this.appId)).pipe(take(1))])
      .subscribe(([gameEnt, newImg]) => {
        this.oldGameEnt = gameEnt;
        this.newGameImg = newImg;
        this.changeDetector.markForCheck();
      })
  }

  public updateAppPhoto(): void {
    const updatedGame = <IGameObj> 
      {name: "admin_component", 
      appId: this.oldGameEnt.appId,
      imageUrl: this.newGameImg  
    }

    this.adminService.updateGameImg(updatedGame).pipe(take(1))
      .subscribe((game: IGameObj) => {
        this.oldGameEnt.imageUrl = game.imageUrl;
        this.changeDetector.markForCheck();
        // TODO: add toast
      });
  }

}
