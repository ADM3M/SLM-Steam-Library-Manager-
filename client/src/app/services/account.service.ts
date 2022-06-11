import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Observable, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { RegisterLoginModalComponent } from '../modals/register-login-modal/register-login-modal.component';
import { IUser } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private baseUrl? = environment.baseUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  public currentUser$ = this.currentUserSource.asObservable();
  
  private registerModal?: BsModalRef;

  constructor(private http: HttpClient, private modalService: BsModalService) { }

  public login(model: any): Observable<void> {
    return this.http.post<IUser>(this.baseUrl + "Account/login", model).pipe(
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
    console.log(user);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    this.registerModal?.hide();
  }

  public logout(): void {
    this.currentUserSource.next(undefined);
    localStorage.removeItem('user');
  }

  getDecodedToken(token: any) {
    return JSON.parse(atob(token.split(".")[1]))
  }

  showLoginModal() {
    const config: ModalOptions = {
      initialState: {
        title: 'Login',
      },

      ignoreBackdropClick: true,
    };

    this.registerModal = this.modalService.show(RegisterLoginModalComponent, config);
  }
}
