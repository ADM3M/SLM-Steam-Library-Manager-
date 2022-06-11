import * as internal from "stream";

export interface IUser {
    id: number;
    username: string;
    steamId: number;
    token: string;
    photoUrl: string;
}
