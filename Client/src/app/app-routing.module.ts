import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin/admin.component';
import { AdminAuthenticateGuard } from './guards/adminAuthenticateGuard';
import { UserAuthenticateGuard } from './guards/userAuthenticateGuard';
import { ApplayoutComponent } from './layouts/applayout/applayout.component';
import { AuthlayoutComponent } from './layouts/authlayout/authlayout.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { TasksComponent } from './tasks/tasks/tasks.component';

const routes: Routes = [{
  path: 'auth', component: AuthlayoutComponent, children: [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent},
    { path: 'register', component: RegisterComponent}
  ]
},{
  path: '', component: ApplayoutComponent, children:[
    { path: '', redirectTo: 'tasks', pathMatch: 'full'},
    { path: 'tasks', component: TasksComponent, canActivate: [UserAuthenticateGuard] },
    { path: 'admin', component: AdminComponent, canActivate: [AdminAuthenticateGuard] }
  ]
}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
