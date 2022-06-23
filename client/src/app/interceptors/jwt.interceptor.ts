import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AccountService } from '../services/account.service';
import { delay, take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private readonly accService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.accService.currentUser$.pipe(take(1)).subscribe(user => {
      if (user && !request.params.has("key")) {
        request = request.clone({
          setHeaders: {
            Authorization: "Bearer " + user.token
          }
        });
      }
      else {
        of(true).pipe(delay(200)).subscribe();
      }
    })


    return next.handle(request);
  }
}
