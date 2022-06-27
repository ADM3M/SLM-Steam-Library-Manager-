import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private readonly accService: AccountService) {}
  
  public canActivate(): Observable<boolean> {
    return this.accService.currentUser$.pipe(take(1), map(user => {
      if (user) {
        return true;
      }

      // TODO: print toast
      console.log("you need to sign in");
      return false;
    }));
  }
  
}
