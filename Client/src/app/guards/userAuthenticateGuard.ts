import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot} from '@angular/router';
import {Observable, of} from 'rxjs';
import {AuthenticationService} from '../services/authentication.service';


@Injectable({
  providedIn: 'root'
})
export class UserAuthenticateGuard implements CanActivate, CanActivateChild {

  constructor(private authenticateService: AuthenticationService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    if (this.authenticateService.isAuth() && this.authenticateService.getCurrentUserRole() == 1) {
      return of(true);
    } else {
      this.router.navigate(['auth/login']);
      return of(false);
    }
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivate(childRoute, state);
  }
}
