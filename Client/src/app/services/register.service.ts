import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { UserRegistration } from '../models/userRegistration';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  constructor(private httpClient: HttpClient) { }

  register(userRegistration: UserRegistration){
    return this.httpClient.post(`${environment.url}api/Registration`, userRegistration);
  }
}
