import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Hostel, HostelApplication, HostelAvailability, Room } from '../models/hostel.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class HostelService{
 constructor(private h:HttpClient){}
 hostels(){return this.h.get<Hostel[]>(`${api}/hostels`)} hostel(id:number){return this.h.get<Hostel>(`${api}/hostels/${id}`)} createHostel(b:unknown){return this.h.post<Hostel>(`${api}/hostels`,b)} updateHostel(id:number,b:unknown){return this.h.put(`${api}/hostels/${id}`,b)} deleteHostel(id:number){return this.h.delete(`${api}/hostels/${id}`)} availability(id:number,academicYear:string,semester:string){return this.h.get<HostelAvailability>(`${api}/hostels/${id}/availability`,{params:{academicYear,semester}})}
 rooms(hostelId?:number){return hostelId?this.h.get<Room[]>(`${api}/hostels/${hostelId}/rooms`):this.h.get<Room[]>(`${api}/rooms`)} createRoom(hostelId:number,b:unknown){return this.h.post<Room>(`${api}/hostels/${hostelId}/rooms`,b)} updateRoom(id:number,b:unknown){return this.h.put(`${api}/rooms/${id}`,b)} deleteRoom(id:number){return this.h.delete(`${api}/rooms/${id}`)} occupancy(id:number){return this.h.get<any>(`${api}/rooms/${id}/occupancy`)}
 apply(b:unknown){return this.h.post<HostelApplication>(`${api}/hostel-applications`,b)} mine(){return this.h.get<HostelApplication[]>(`${api}/hostel-applications/me`)} applications(status?:string,page=1){let p=new HttpParams().set('page',page);if(status)p=p.set('status',status);return this.h.get<HostelApplication[]>(`${api}/hostel-applications`,{params:p})} cancel(id:number){return this.h.delete(`${api}/hostel-applications/${id}`)} updateStatus(id:number,status:string){return this.h.put<HostelApplication>(`${api}/hostel-applications/${id}/status`,{status})} assignRoom(id:number,roomId:number){return this.h.put<HostelApplication>(`${api}/hostel-applications/${id}/assign-room`,{roomId})} unassignRoom(id:number){return this.h.put(`${api}/hostel-applications/${id}/unassign-room`,{})}
}
