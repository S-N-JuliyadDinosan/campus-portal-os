import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, finalize, map, shareReplay, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, JwtUser, UserRole } from '../models/api.models';

interface LoginResponse { accessToken:string; refreshToken:string; accessTokenExpiresAt:string; }

@Injectable({providedIn:'root'})
export class AuthService {
  private readonly api = `${environment.apiUrl}/auth`;
  private accessToken = signal<string | null>(sessionStorage.getItem('csp_access'));
  private refreshToken = signal<string | null>(sessionStorage.getItem('csp_refresh'));
  private refreshInFlight$: Observable<LoginResponse> | null = null;

  readonly user = computed<JwtUser>(() => this.decode(this.accessToken()));
  readonly isAuthenticated = computed(() => !!this.accessToken() && !!this.user().role);
  readonly role = computed<UserRole | null>(() => this.user().role);
  readonly studentId = computed(() => this.user().studentId);

  constructor(private http:HttpClient, private router:Router) {}

  login(email:string,password:string):Observable<LoginResponse>{
    return this.http.post<ApiResponse<LoginResponse>>(`${this.api}/login`,{email,password}).pipe(
      map(r=>this.unwrap(r)), tap(r=>this.save(r))
    );
  }

  refresh():Observable<LoginResponse>{
    if(this.refreshInFlight$) return this.refreshInFlight$;
    const token=this.refreshToken();
    if(!token) return throwError(()=>new Error('No refresh token is available.'));
    this.refreshInFlight$=this.http.post<ApiResponse<LoginResponse>>(`${this.api}/refresh`,{refreshToken:token}).pipe(
      map(r=>this.unwrap(r)), tap(r=>this.save(r)),
      finalize(()=>this.refreshInFlight$=null), shareReplay({bufferSize:1,refCount:false})
    );
    return this.refreshInFlight$;
  }

  logout():void{
    const refreshToken=this.refreshToken();
    if(refreshToken){ this.http.post(`${this.api}/logout`,{refreshToken}).subscribe({error:()=>{}}); }
    this.clear();
    void this.router.navigate(['/login']);
  }

  logoutAll(){ return this.http.post(`${this.api}/logout-all`,{}); }
  changePassword(currentPassword:string,newPassword:string){ return this.http.post(`${this.api}/change-password`,{currentPassword,newPassword}); }
  forgotPassword(email:string){ return this.http.post(`${this.api}/forgot-password`,{email}); }
  resetPassword(token:string,newPassword:string){ return this.http.post(`${this.api}/reset-password`,{token,newPassword}); }
  verifyEmail(email:string,token:string){ return this.http.post(`${this.api}/verify-email`,{email,token}); }
  resendVerification(email:string){ return this.http.post(`${this.api}/resend-verification`,{email}); }
  token(){ return this.accessToken(); }
  refreshValue(){ return this.refreshToken(); }
  clear(){ this.accessToken.set(null); this.refreshToken.set(null); sessionStorage.removeItem('csp_access'); sessionStorage.removeItem('csp_refresh'); }

  private unwrap<T>(r:ApiResponse<T>):T{
    if(!r.success || r.data==null) throw new Error(r.message || 'Request failed.');
    return r.data;
  }
  private save(r:LoginResponse){
    this.accessToken.set(r.accessToken); this.refreshToken.set(r.refreshToken);
    sessionStorage.setItem('csp_access',r.accessToken); sessionStorage.setItem('csp_refresh',r.refreshToken);
  }
  private decode(token:string|null):JwtUser{
    const empty:JwtUser={userId:null,studentId:null,email:'',role:null};
    if(!token) return empty;
    try{
      const segment=token.split('.')[1];
      if(!segment) return empty;
      const normalized=segment.replace(/-/g,'+').replace(/_/g,'/').padEnd(Math.ceil(segment.length/4)*4,'=');
      const body=JSON.parse(atob(normalized));
      const role=(body.role ?? body['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']) as UserRole | undefined;
      const email=body.email ?? body['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '';
      const uid=body.userId ?? body.nameid ?? body['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      return { userId:uid?Number(uid):null, studentId:body.studentId?Number(body.studentId):null, email, role:role==='Admin'||role==='Student'?role:null };
    } catch { return empty; }
  }
}
