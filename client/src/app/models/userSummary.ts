import { IGameObj } from "./gameObj";

export interface IUserSummary {
    lastGames: IGameObj[];
    total: number;
    notSet: number;
    inProgress: number;
    completed: number;
    backlog: number;
}