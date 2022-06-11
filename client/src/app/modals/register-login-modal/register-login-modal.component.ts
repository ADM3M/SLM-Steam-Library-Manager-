import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-register-login-modal',
  templateUrl: './register-login-modal.component.html',
  styleUrls: ['./register-login-modal.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class RegisterLoginModalComponent implements OnInit {
  public title?: string;
  public login = "";
  public password = "";

 
  constructor(public bsModalRef: BsModalRef, accService: AccountService) {}
 
  ngOnInit() {
  }

  public logIn(model: any) {

  }
}
