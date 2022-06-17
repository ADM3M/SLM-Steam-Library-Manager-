export class SortObj {
    constructor(public name: string, public value: boolean = false, public reverse: boolean = false) {
    }

    public reverseVal(): void {
        if (this.value === false) {
            this.value = true;
            return;
        }

        this.reverse = !this.reverse;
    }

    public reset(): void {
        this.value = false;
        this.reverse = false;
    }
}