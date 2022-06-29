import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { IUser } from '../models/user';
import { AccountService } from '../services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  private user: IUser;

  constructor(private viewContainerRef: ViewContainerRef,
    private tempaltesRef: TemplateRef<any>,
    private accService: AccountService) {
    this.accService.currentUser$.pipe(take(1)).subscribe((user: IUser) => {
      this.user = user;
    });
  }
  ngOnInit(): void {
    if (!this.user?.role || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    if (this.user?.role.some(e => this.appHasRole.includes(e))) {
      this.viewContainerRef.createEmbeddedView(this.tempaltesRef);
    }
    else {
      this.viewContainerRef.clear();
    }
  }
}
