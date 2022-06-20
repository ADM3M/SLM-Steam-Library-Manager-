import { GameState } from "../enums/gameState";

export interface IGameObj {
    gameId: number;
    appId: number;
    name: string;
    iconUrl: string;
    imageUrl: string;
    userPlayTime: number;
    status: GameState;
}