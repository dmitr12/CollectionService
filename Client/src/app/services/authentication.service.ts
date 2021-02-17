import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import jwt_decode from 'jwt-decode';
import { environment } from 'src/environments/environment';
import { Token, TokenMessage } from '../models/token';
import { UserAuthentication } from '../models/userAuthentication';
import jwtDecode from 'jwt-decode';

export const TOKEN = '';

export function getToken(){
  return localStorage.getItem(TOKEN)
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpClient: HttpClient,
    private jwjtHelper: JwtHelperService,
    private router: Router) { }

  login(userAuth: UserAuthentication): Observable<TokenMessage> {
    return this.httpClient.post<TokenMessage>(`${environment.url}api/authentication`, userAuth).pipe(tap(
      token => {
        localStorage.setItem(TOKEN, token.token);
      }
    ))
  }

  getCurrentUserName() {
    return jwt_decode<Token>(localStorage.getItem(TOKEN)!).login;
  }

  getCurrentUserId() {
    return jwt_decode<Token>(localStorage.getItem(TOKEN)!).sub;
  }

  isAuth(): any {
    let token = localStorage.getItem(TOKEN);
    return token && !this.jwjtHelper.isTokenExpired(token);
  }

  logout() {
    localStorage.removeItem(TOKEN);
    this.router.navigate(['auth']);
  }
}
