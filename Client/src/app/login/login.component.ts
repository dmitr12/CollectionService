import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {UserAuthentication} from '../models/userAuthentication';
import {AuthenticationService} from '../services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  formLogin = new FormGroup({
    login: new FormControl(null, [Validators.required]),
    password: new FormControl(null, [Validators.required])
  });

  constructor(private authenticationService: AuthenticationService, private router: Router) {
  }

  ngOnInit(): void {

  }

  login() {
    this.authenticationService.login(new UserAuthentication(this.formLogin.value.login, this.formLogin.value.password)).subscribe(res => {
      if (this.authenticationService.getCurrentUserRole() == 1) {
        this.router.navigate(['']);
      } else if (this.authenticationService.getCurrentUserRole() == 2) {
        this.router.navigate(['/admin']);
      }
    }, error => {
      if (error.status == 401) {
        alert('Неверный логин или пароль');
      }
      else {
        alert('Возникла ошибка, статусный код ' + error.status);
      }
    });
  }
}
