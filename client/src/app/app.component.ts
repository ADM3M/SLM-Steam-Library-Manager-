import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';
import { IUser } from './models/user';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  title = 'client';

  constructor(private modalService: BsModalService, public accService: AccountService) {}
  
  ngOnInit(): void {
    const userJson = localStorage.getItem('user');  
    
    if (userJson) {
      const user: IUser = JSON.parse(userJson);
      this.accService.setCurrentUser(user);
      return;
    }
    
    this.accService.showLoginModal();
  }

}
