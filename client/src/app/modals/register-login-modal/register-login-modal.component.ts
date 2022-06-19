import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { take } from 'rxjs/operators';
import { ISteamUser } from 'src/app/models/steamUser';
import { IUser } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { MemberService } from 'src/app/services/member.service';
import { SteamService } from 'src/app/services/steam.service';

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

  constructor(public readonly bsModalRef: BsModalRef, private readonly accService: AccountService, private readonly fb: FormBuilder,
     private readonly steamService: SteamService,
     private readonly memberService: MemberService) { }

  ngOnInit() {
  }

  public loginUser(): void {
    this.accService.login(this.reactiveForm?.value).subscribe(user => {
      this.fetchSteamImage(user);
    });
  }

  public registerUser(): void {
    this.accService.register(this.reactiveForm?.value).subscribe(user => {
      this.fetchSteamImage(user);
  });
  }

  fetchSteamImage(user: IUser): void {
    
    if(user.photoUrl || !user.steamId) {
      return;
    }
    
    this.steamService.getMemberProfileInfo(user.steamId)
      .pipe(take(1))
      .subscribe((steamUser: ISteamUser) => {
        const updatedUser = user;
        updatedUser.photoUrl = steamUser.avatarmedium;
        this.accService.setCurrentUser(updatedUser);
        this.memberService.updateSteamUserData(steamUser).subscribe();
      })
  }
}
