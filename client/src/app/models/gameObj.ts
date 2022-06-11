import { GameState } from "../enums/gameState";

export interface IGameObj {
    name: string;
    iconUrl: string;
    pictureUrl: string;
    totalPlayed: number;
    status: GameState;
}