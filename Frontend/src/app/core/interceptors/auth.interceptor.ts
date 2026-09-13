import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

const publicAuthPaths=['/auth/login','/auth/refresh','/auth/forgot-password','/auth/reset-password','/auth/verify-email','/auth/resend-verification','/students/register'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth=inject(AuthService);
  const router=inject(Router);
  const isPublic=publicAuthPaths.some(path=>req.url.includes(path));
  const token=auth.token();
  const authorizedReq=!isPublic&&token?req.clone({setHeaders:{Authorization:`Bearer ${token}`}}):req;

  return next(authorizedReq).pipe(catchError((error:unknown)=>{
    if(!(error instanceof HttpErrorResponse) || error.status!==401 || isPublic || !auth.refreshValue()){
      return throwError(()=>error);
    }
    return auth.refresh().pipe(
      switchMap(()=>{
        const fresh=auth.token();
        return next(fresh?req.clone({setHeaders:{Authorization:`Bearer ${fresh}`}}):req);
      }),
      catchError(refreshError=>{
        auth.clear();
        void router.navigate(['/login'],{queryParams:{reason:'session-expired'}});
        return throwError(()=>refreshError);
      })
    );
  }));
};
