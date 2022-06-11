import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { IUser } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private baseUrl?: string;
  private currentUserSource = new ReplaySubject<IUser>(1);
  public currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  public login(model: any): Observable<void> {
    return this.http.post<IUser>(this.baseUrl + "account/login", model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  public register(model: any): Observable<void> {
    return this.http.post<IUser>(this.baseUrl + "account/register", model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  public setCurrentUser(user: IUser): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  public logout(): void {
    this.currentUserSource.next(undefined);
    localStorage.removeItem('user');
  }

  getDecodedToken(token: any) {
    return JSON.parse(atob(token.split(".")[1]))
  }
}
