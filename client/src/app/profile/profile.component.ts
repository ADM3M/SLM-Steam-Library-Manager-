import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { forkJoin, Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { ISteamUser } from '../models/steamUser';
import { IUser } from '../models/user';
import { IUserSummary } from '../models/userSummary';
import { AccountService } from '../services/account.service';
import { MemberService } from '../services/member.service';
import { SteamService } from '../services/steam.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  summary: IUserSummary;
  active: any[];
  public reactiveForm: FormGroup = this.fb.group({
    "steamUrl": ["", [Validators.required]]
  })

  constructor(public readonly memberService: MemberService,
    private readonly changeDetection: ChangeDetectorRef,
    private readonly fb: FormBuilder,
    private readonly accService: AccountService,
    private readonly steamService: SteamService) { }

  ngOnInit(): void {
    this.initPlayerSummary();
  }

  public initPlayerSummary(): void {
    this.memberService.getUserSummaries().pipe(
      take(1),
      map(summary => {
        this.summary = summary;
        this.changeDetection.markForCheck();
      })).subscribe(() => {
        this.updateActive(this.summary.inProgress, "warning");
      });
  }

  public setActive(name: string): void {
    switch (name) {
      case "inProgress":
        this.updateActive(this.summary.inProgress, "warning");
        break;

      case "completed":
        this.updateActive(this.summary.completed, "success");
        break;

      case "backlog":
        this.updateActive(this.summary.backlog, "info");
        break;

      case "notSet":
        this.updateActive(this.summary.notSet, "danger");
        break;

      default:
        break;
    }
  }

  public updateActive(value: number, type: string): void {
    this.active = [{ value: value, type: type }];
  }

  public fetchSteamImage(user: IUser): Observable<IUser> {
    return this.steamService.getMemberProfileInfo(user.steamId)
      .pipe(
        take(1),
        map((steamUser: ISteamUser) => {
          user.photoUrl = steamUser.avatarmedium;
          return user;
        }));
  }

  public updateSteamId(): void {
    forkJoin([this.accService.currentUser$.pipe(take(1)),
    this.memberService.getUserIdByUrl(this.reactiveForm?.value?.steamUrl).pipe(take(1))])
      .pipe(take(1), map(([acc, steamId]) => {
        if (acc.steamId == steamId) {
          return;
          // TODO: toast
        }

        acc.steamId = steamId;

        this.fetchSteamImage(acc).pipe(
          take(1),
          map(updatedUser => {
            this.memberService.updateSteamUserData(<ISteamUser>{ steamid: steamId, avatarmedium: updatedUser.photoUrl }).subscribe(() => {
              acc.photoUrl = updatedUser.photoUrl;
              this.accService.setCurrentUser(acc);
              this.memberService.isFetchNeeded = true;
            })
          })
        ).subscribe();

      })).subscribe();
  }
}
