import { GameState } from "../enums/gameState";

export interface IGameObj {
    id: number;
    appId: number;
    name: string;
    iconUrl: string;
    imageUrl: string;
    userPlayTime: number;
    status: GameState;
}