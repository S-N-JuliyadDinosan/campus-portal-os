import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
export const adminGuard:CanActivateFn=()=>{const a=inject(AuthService);return a.role()==='Admin'?true:inject(Router).createUrlTree(['/dashboard']);};
