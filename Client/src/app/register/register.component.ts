import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserRegistration } from '../models/userRegistration';
import { RegisterService } from '../services/register.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  
  formRegister: FormGroup = new FormGroup({
    userName: new FormControl(null, [Validators.required, Validators.maxLength(100), this.validName]),
    email: new FormControl(null, [Validators.required, Validators.email]),
    passwords: new FormGroup({
      password: new FormControl(null, [Validators.required, Validators.minLength(6), Validators.maxLength(50)]),
      confirmPassword: new FormControl(null, [Validators.required, Validators.minLength(6), Validators.maxLength(50)])
    })
  }, this.matchingPasswords);

  constructor(private registerService: RegisterService, private router: Router) { }

  matchingPasswords(c: AbstractControl): ValidationErrors | null {
    let password = c.get('passwords.password');
    let confirmPassword = c.get('passwords.confirmPassword');
    if (password?.value !== confirmPassword?.value) {
      return { custom: 'Пароли не равны' };
    }
    return null;
  }

  validName(c: FormControl): ValidationErrors | null{
    if (c?.value?.toString().includes('@')) {
      return { custom: 'Недопустимый символ @' };
    }
    return null;
  }

  ngOnInit(): void {

  }

  onSubmit(){
    this.registerService.register(new UserRegistration(this.formRegister.value.userName, this.formRegister.value.email,
      this.formRegister.value.passwords.password)).subscribe((res:any)=>{
        if (res){
          alert(res['msg'])
        }
        else{
          this.router.navigate(['']);
        }
      }, error => {
        alert("При отправке запроса возникла ошибка, статусный код "+error.status);
      });
  }

}
