import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Complaint, ComplaintCategory } from '../models/complaint.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class ComplaintService{
 constructor(private h:HttpClient){} categories(){return this.h.get<ComplaintCategory[]>(`${api}/complaint-categories`)} createCategory(b:unknown){return this.h.post<ComplaintCategory>(`${api}/complaint-categories`,b)} updateCategory(id:number,b:unknown){return this.h.put<ComplaintCategory>(`${api}/complaint-categories/${id}`,b)} deleteCategory(id:number){return this.h.delete(`${api}/complaint-categories/${id}`)} create(b:unknown){return this.h.post<Complaint>(`${api}/complaints`,b)} mine(){return this.h.get<Complaint[]>(`${api}/complaints/me`)} all(filters:Record<string,string|number|undefined>={}){let p=new HttpParams();Object.entries(filters).forEach(([k,v])=>{if(v!==undefined&&v!=='')p=p.set(k,String(v))});return this.h.get<Complaint[]>(`${api}/complaints`,{params:p})} updateStatus(id:number,status:number,resolutionNote?:string){return this.h.put<Complaint>(`${api}/complaints/${id}/status`,{status,resolutionNote})}}
