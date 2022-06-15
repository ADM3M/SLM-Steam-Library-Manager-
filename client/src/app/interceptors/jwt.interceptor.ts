import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { take } from 'rxjs/operators';

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
    })


    return next.handle(request);
  }
}
