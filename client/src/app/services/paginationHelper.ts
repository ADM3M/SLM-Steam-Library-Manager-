import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../models/pagination";

export function getPaginatedResult<T>(url: string, http: HttpClient, params: HttpParams): Observable<PaginatedResult<T>> {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    return http.get<T>(url, { observe: 'response', params: params }).pipe(
        map(response => {
            paginatedResult.result = response?.body;
            if (response.headers.get("pagination") !== null) {
                paginatedResult.pagination = JSON.parse(response.headers.get("pagination")!);
            }

            return paginatedResult;
        })
    );
}

export function getPaginationHeader(pageNumber: number): HttpParams {
    let httpParams = new HttpParams();

    httpParams = httpParams
        .append('pageNumber', pageNumber.toString())

    return httpParams;
}