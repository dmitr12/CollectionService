import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JwtModule } from '@auth0/angular-jwt';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthlayoutComponent } from './layouts/authlayout/authlayout.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { getToken, TOKEN } from './services/authentication.service';
import { ApplayoutComponent } from './layouts/applayout/applayout.component';
import { TasksComponent } from './tasks/tasks/tasks.component';
import { MatDialogModule } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AddTaskComponent } from './tasks/add-task/add-task.component';
import {MatSelectModule} from '@angular/material/select';
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker';
import { environment } from 'src/environments/environment';
import {DatePipe} from '@angular/common';
import { DeleteTaskComponent } from './tasks/delete-task/delete-task.component';

@NgModule({
  declarations: [
    AppComponent,
    AuthlayoutComponent,
    LoginComponent,
    RegisterComponent,
    ApplayoutComponent,
    TasksComponent,
    AddTaskComponent,
    DeleteTaskComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    MatDialogModule,
    MatSelectModule,
    NgxMaterialTimepickerModule,
    HttpClientModule,
    JwtModule.forRoot({
      config:{
        tokenGetter: getToken,
        allowedDomains: environment.allowedDomains
      }
    }),
    BrowserAnimationsModule
  ],
  providers: [DatePipe],
  bootstrap: [AppComponent],
  entryComponents: [AddTaskComponent]
})
export class AppModule { }
