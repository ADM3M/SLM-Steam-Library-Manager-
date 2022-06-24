export class DisplayParams {
    search: string = "";
    statusesToShow: string = "012";
    orderBy: string = "playedTimeReverse";

    join(): string {
        return `${this.orderBy}-${this.search}-${this.statusesToShow}`;
    }
}