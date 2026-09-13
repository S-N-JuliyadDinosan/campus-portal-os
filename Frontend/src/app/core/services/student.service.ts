import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api.models';
import { AdminAccount, Faculty, StudentActivitySummary, StudentListItem, StudentMaster, StudentMasterCheck, StudentProfile } from '../models/domain.models';
@Injectable({providedIn:'root'})
export class StudentService {
  private api=environment.apiUrl;
  constructor(private http:HttpClient){}
  private unwrap<T>(){return map((r:ApiResponse<T>)=>{if(!r.success||r.data==null) throw new Error(r.message); return r.data;});}
  register(body:unknown){return this.http.post<ApiResponse<unknown>>(`${this.api}/students/register`,body);}
  checkMaster(index:string){return this.http.get<ApiResponse<StudentMasterCheck>>(`${this.api}/student-master/${encodeURIComponent(index)}`).pipe(this.unwrap<StudentMasterCheck>());}
  me(){return this.http.get<ApiResponse<StudentProfile>>(`${this.api}/students/me`).pipe(this.unwrap<StudentProfile>());}
  updateMe(body:{phoneNumber?:string|null;address?:string|null}){return this.http.put<ApiResponse<StudentProfile>>(`${this.api}/students/me`,body).pipe(this.unwrap<StudentProfile>());}
  activity(){return this.http.get<ApiResponse<StudentActivitySummary>>(`${this.api}/students/me/activity-summary`).pipe(this.unwrap<StudentActivitySummary>());}
  search(search='',facultyId?:number,page=1){let p=new HttpParams().set('page',page);if(search)p=p.set('search',search);if(facultyId)p=p.set('facultyId',facultyId);return this.http.get<ApiResponse<PagedResult<StudentListItem>>>(`${this.api}/students`,{params:p}).pipe(this.unwrap<PagedResult<StudentListItem>>());}
  get(id:number){return this.http.get<any>(`${this.api}/students/${id}`);}
  adminUpdate(id:number,body:unknown){return this.http.put<ApiResponse<StudentProfile>>(`${this.api}/admin/students/${id}`,body).pipe(this.unwrap<StudentProfile>());}
  deactivationCheck(id:number){return this.http.get<ApiResponse<{canDeactivate:boolean;blockingCommitments:string[]}>>(`${this.api}/admin/students/${id}/deactivation-check`).pipe(this.unwrap<{canDeactivate:boolean;blockingCommitments:string[]}>());}
  deactivate(id:number){return this.http.put(`${this.api}/admin/students/${id}/deactivate`,{});}
  reactivate(id:number){return this.http.put(`${this.api}/admin/students/${id}/reactivate`,{});}
  faculties(){return this.http.get<ApiResponse<Faculty[]>>(`${this.api}/faculties`).pipe(this.unwrap<Faculty[]>());}
  createFaculty(body:unknown){return this.http.post<ApiResponse<Faculty>>(`${this.api}/faculties`,body).pipe(this.unwrap<Faculty>());}
  updateFaculty(id:number,body:unknown){return this.http.put<ApiResponse<Faculty>>(`${this.api}/faculties/${id}`,body).pipe(this.unwrap<Faculty>());}
  deleteFaculty(id:number){return this.http.delete(`${this.api}/faculties/${id}`);}
  masterSearch(search='',facultyId?:number,page=1){let p=new HttpParams().set('page',page);if(search)p=p.set('search',search);if(facultyId)p=p.set('facultyId',facultyId);return this.http.get<ApiResponse<PagedResult<StudentMaster>>>(`${this.api}/student-master`,{params:p}).pipe(this.unwrap<PagedResult<StudentMaster>>());}
  createMaster(body:unknown){return this.http.post<ApiResponse<StudentMaster>>(`${this.api}/student-master`,body).pipe(this.unwrap<StudentMaster>());}
  updateMaster(id:number,body:unknown){return this.http.put<ApiResponse<StudentMaster>>(`${this.api}/student-master/${id}`,body).pipe(this.unwrap<StudentMaster>());}
  deleteMaster(id:number){return this.http.delete(`${this.api}/student-master/${id}`);}
  importMaster(file:File){const fd=new FormData();fd.append('file',file);return this.http.post<ApiResponse<any>>(`${this.api}/student-master/import`,fd).pipe(this.unwrap<any>());}
  admins(){return this.http.get<ApiResponse<AdminAccount[]>>(`${this.api}/admin/admins`).pipe(this.unwrap<AdminAccount[]>());}
  createAdmin(body:unknown){return this.http.post<ApiResponse<AdminAccount>>(`${this.api}/admin/admins`,body).pipe(this.unwrap<AdminAccount>());}
  updateAdmin(id:number,email:string){return this.http.put<ApiResponse<AdminAccount>>(`${this.api}/admin/admins/${id}`,{email}).pipe(this.unwrap<AdminAccount>());}
  deactivateAdmin(id:number){return this.http.put(`${this.api}/admin/admins/${id}/deactivate`,{});}
  reactivateAdmin(id:number){return this.http.put(`${this.api}/admin/admins/${id}/reactivate`,{});}
}
