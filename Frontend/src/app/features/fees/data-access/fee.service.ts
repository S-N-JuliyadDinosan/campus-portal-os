import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { FeePayment, FeeReceipt, FeeType } from '../models/fee.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class FeeService{
 constructor(private h:HttpClient){} types(){return this.h.get<FeeType[]>(`${api}/fees/types`)} createType(b:unknown){return this.h.post<FeeType>(`${api}/fees/types`,b)} updateType(id:number,b:unknown){return this.h.put(`${api}/fees/types/${id}`,b)} deleteType(id:number){return this.h.delete(`${api}/fees/types/${id}`)} mine(){return this.h.get<FeePayment[]>(`${api}/fees/me`)} unpaid(){return this.h.get<FeePayment[]>(`${api}/fees/me/unpaid`)} paid(){return this.h.get<FeePayment[]>(`${api}/fees/me/paid`)} pay(id:number){return this.h.post(`${api}/fees/${id}/simulate-payment`,{})} receipt(id:number){return this.h.get<FeeReceipt>(`${api}/fee-payments/${id}/receipt`)} all(){return this.h.get<FeePayment[]>(`${api}/fees`)} assign(b:unknown){return this.h.post(`${api}/fees/assign`,b)} updateStatus(id:number,status:string){return this.h.put(`${api}/fees/${id}/status`,{status})} delete(id:number){return this.h.delete(`${api}/fees/${id}`)} }
