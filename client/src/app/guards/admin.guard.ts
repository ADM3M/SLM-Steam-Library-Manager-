import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { IUser } from '../models/user';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private readonly accService: AccountService) { }

  canActivate(): Observable<boolean> {
    return this.accService.currentUser$.pipe(take(1),
      map((user: IUser) => {
        return user.role.includes("admin");
      }))
  }

}
