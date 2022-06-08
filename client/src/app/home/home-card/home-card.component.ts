import { Component, Input, OnInit } from '@angular/core';
import { GameState } from '../../enums/gameState';
import { IGameObj } from '../models/gameObj';

@Component({
  selector: 'app-home-card',
  templateUrl: './home-card.component.html',
  styleUrls: ['./home-card.component.css']
})
export class HomeCardComponent implements OnInit {

  @Input() gameData: IGameObj = {
    name: "Unknown", 
    pictureUrl : './assets/unknownImg.jpg',
    totalPlayed : 0,
    status : GameState.notSet,
    iconUrl : "'./assets/unknownIcon.png'" 
  };

  constructor() { }

  ngOnInit(): void {
  }

  public imgStyles: any = {
    'background-image': 'url(' + this.gameData.pictureUrl + ')',
    'background-size': 'contain',
    'background-position': 'center',
    'background-repeat': 'no-repeat'
  };

  public imgBlurStyles: any = {
    'background-image': 'url(' + this.gameData.pictureUrl + ')',
    'background-position': 'center',
    'background-repeat': 'no-repeat',
    'background-size': '100% auto'
  };

  public thumbStyles: any = {
    'background': 'url(' + this.gameData?.iconUrl || "" + ')',
    'background-size': 'contain',
  }

}
