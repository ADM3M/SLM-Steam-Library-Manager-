import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router'
import { AdminComponent } from './admin/admin/admin.component';
import { CrutchComponent } from './crutch/crutch.component';
import { AdminGuard } from './guards/admin.guard';
import { AuthGuard } from './guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';

const routes: Routes = [
  {
    path: "", runGuardsAndResolvers: "always", children: [
      { path: "profile", canActivate: [AuthGuard], component: ProfileComponent },
      { path: "admin", canActivate: [AdminGuard], component: AdminComponent },
      { path: "", component: HomeComponent }
    ]
  },
  { path: "crutch", component: CrutchComponent },
  { path: "**", component: HomeComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }