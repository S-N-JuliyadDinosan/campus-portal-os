import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { CertificateRequest, CertificateType } from '../models/certificate.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class CertificateService{
 constructor(private h:HttpClient){} types(){return this.h.get<CertificateType[]>(`${api}/certificate-types`)} createType(b:unknown){return this.h.post<CertificateType>(`${api}/certificate-types`,b)} updateType(id:number,b:unknown){return this.h.put(`${api}/certificate-types/${id}`,b)} deleteType(id:number){return this.h.delete(`${api}/certificate-types/${id}`)} create(certificateTypeId:number,reason?:string){return this.h.post<CertificateRequest>(`${api}/certificate-requests`,{certificateTypeId,reason})} mine(){return this.h.get<CertificateRequest[]>(`${api}/certificate-requests/me`)} all(status?:string,page=1){let p=new HttpParams().set('page',page);if(status)p=p.set('status',status);return this.h.get<CertificateRequest[]>(`${api}/certificate-requests`,{params:p})} updateStatus(id:number,status:string,reviewNote?:string){return this.h.put<CertificateRequest>(`${api}/certificate-requests/${id}/status`,{status,reviewNote})}}
