import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { IGameObj } from 'src/app/models/gameObj';
import { MemberService } from 'src/app/services/member.service';
import { SteamService } from 'src/app/services/steam.service';
import { GameState } from '../../enums/gameState';

@Component({
  selector: 'app-home-card',
  templateUrl: './home-card.component.html',
  styleUrls: ['./home-card.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeCardComponent implements OnInit {
  @Input() gameData: IGameObj = {
    appId: 0, iconUrl: "", gameId: 0, name: "",
    imageUrl: "", status: GameState.NotSet, userPlayTime: 0
  }

  public imgStyles: any;
  public imgBlurStyles: any;
  public thumbStyles: any;
  public menuHide = true;

  constructor(private readonly memberService: MemberService,
    private readonly steamService: SteamService) { }

  ngOnInit(): void {
    this.fetchGameImg();
    this.replaceMissingPicture();
    this.InitializeStyles();
  }

  public fetchGameImg(): void {
    if (this.gameData.imageUrl && this.gameData.imageUrl !== "./assets/unknownImg.jpg") {
      return;
    }

    this.steamService.getGameImg(this.gameData.appId).pipe(take(1))
      .subscribe((imgUrl: string) => {
        console.log(`new image: ${imgUrl}`);
        this.gameData.imageUrl = imgUrl;
        this.memberService.updateGame(this.gameData).subscribe();
        // TODO: toasts
      }, err => {console.log(err)});
  }

  private InitializeStyles(): void {
    this.imgStyles = {
      'background-image': 'url(' + this.gameData.imageUrl || "./assets/unknownImg.jpg" + ')',
      'background-size': 'contain',
      'background-position': 'center',
      'background-repeat': 'no-repeat'
    };

    this.imgBlurStyles = {
      'background-image': 'url(' + this.gameData.imageUrl || "./assets/unknownImg.jpg" + ')',
      'background-position': 'center',
      'background-repeat': 'no-repeat',
      'background-size': 'cover'
    };

    this.thumbStyles = {
      'background-image': 'url(' + this.gameData.iconUrl + ')',
      'background-size': 'contain',
    };
  }

  private replaceMissingPicture(): void {
    if (!this.gameData?.imageUrl) {
      this.gameData.imageUrl = "./assets/unknownImg.jpg";
    }

    if (!this.gameData?.iconUrl) {
      this.gameData.iconUrl = './assets/unknownImg.jpg'
    }
  }

  public getEnumName(): string {
    const value = this.gameData.status;
    let result = "";

    switch (value) {
      case 0:
        result = "not set";
        break;

      case 1:
        result = "in progress";
        break;

      case 2:
        result = "completed";
        break;

      case 3:
        result = "backlog";
        break;
    }

    return result;
  }

  public ChangeStatus(status: number): void {
    if (this.gameData.status == status) {
      this.menuHide = true;
      return;
    }

    this.gameData.status = status;
    this.menuHide = true;
    this.memberService.updateGameStatus(this.gameData).subscribe();
  }

  public getFontSizeByLength(length: number): string {
    const coef = Math.round(length / 10);
    return `${1.2 - (coef / 10)}`;
  }
}
