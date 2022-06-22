import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  public loginMode = true;
  public title?: string;
  public reactiveForm: FormGroup = this.fb.group({
    "username": ["", [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
    "password": ["", [Validators.required, Validators.minLength(4), Validators.maxLength(32)]]
  })

  constructor(public readonly bsModalRef: BsModalRef,
    private readonly accService: AccountService,
    private readonly fb: FormBuilder) { }

  ngOnInit() {
  }

  public loginUser(): void {
    this.accService.login(this.reactiveForm?.value).subscribe();
  }

  public registerUser(): void {
    this.accService.register(this.reactiveForm?.value).subscribe();
  }
}
