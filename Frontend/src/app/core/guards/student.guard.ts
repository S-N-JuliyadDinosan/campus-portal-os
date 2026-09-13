import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
export const studentGuard:CanActivateFn=()=>{const a=inject(AuthService);return a.role()==='Student'?true:inject(Router).createUrlTree(['/dashboard']);};
