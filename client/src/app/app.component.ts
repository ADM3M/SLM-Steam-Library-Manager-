import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RegisterLoginModalComponent } from './modals/register-login-modal/register-login-modal.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  title = 'client';
  private bsModalRef?: BsModalRef;

  constructor(private modalService: BsModalService) {}
  
  ngOnInit(): void {
    this.showModal();
  }
  
  showModal() {
    const config: ModalOptions = {
      initialState: {
        title: 'Login',
      },

      ignoreBackdropClick: true,
    };

    this.bsModalRef = this.modalService.show(RegisterLoginModalComponent, config);
    this.bsModalRef.content.closeBtnName = 'Close';
  }
}
