import { Component, Input, OnInit } from '@angular/core';
import { IGameObj } from 'src/app/models/gameObj';
import { MemberService } from 'src/app/services/member.service';
import { GameState } from '../../enums/gameState';

@Component({
  selector: 'app-home-card',
  templateUrl: './home-card.component.html',
  styleUrls: ['./home-card.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeCardComponent implements OnInit {
  @Input() gameData: IGameObj = { appId: 0, iconUrl: "", gameId: 0, name: "",
    imageUrl: "", status: GameState.NotSet, userPlayTime: 0 }

  public imgStyles: any;
  public imgBlurStyles: any;
  public thumbStyles: any;
  public menuHide = true;

  constructor(private readonly memberService: MemberService) { }

  ngOnInit(): void {
    this.replaceMissingPicture();
    this.InitializeStyles();
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
      'background-size': '100% auto'
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
        result = "forgotten";
        break;
    }

    return result;
  }

  public ChangeStatus(status: number) {
    if (this.gameData.status == status) {
      this.menuHide = true;
      return;
    }

    this.gameData.status = status;
    this.menuHide = true;
    this.memberService.updateGameStatus(this.gameData).subscribe();
  }
}
