import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Lab, LabBooking, LabSeat, LabSlot } from '../models/lab.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class LabService{
 constructor(private h:HttpClient){}
 labs(){return this.h.get<Lab[]>(`${api}/labs`)} createLab(b:unknown){return this.h.post<Lab>(`${api}/labs`,b)} updateLab(id:number,b:unknown){return this.h.put(`${api}/labs/${id}`,b)} deleteLab(id:number){return this.h.delete(`${api}/labs/${id}`)}
 slots(labId:number,date:string){return this.h.get<LabSlot[]>(`${api}/labs/${labId}/slots`,{params:{date}})} createSlot(labId:number,b:unknown){return this.h.post<LabSlot>(`${api}/labs/${labId}/time-slots`,b)} updateSlot(id:number,b:unknown){return this.h.put(`${api}/lab-time-slots/${id}`,b)} deleteSlot(id:number){return this.h.delete(`${api}/lab-time-slots/${id}`)}
 seats(labId:number){return this.h.get<LabSeat[]>(`${api}/labs/${labId}/seats`)} availableSeats(labId:number,date:string,timeSlotId:number){return this.h.get<LabSeat[]>(`${api}/labs/${labId}/seats/availability`,{params:{date,timeSlotId}})} createSeat(labId:number,b:unknown){return this.h.post<LabSeat>(`${api}/labs/${labId}/seats`,b)} updateSeat(labId:number,id:number,b:unknown){return this.h.put(`${api}/labs/${labId}/seats/${id}`,b)} deleteSeat(labId:number,id:number){return this.h.delete(`${api}/labs/${labId}/seats/${id}`)}
 book(b:unknown){return this.h.post<LabBooking>(`${api}/lab-bookings`,b)} mine(){return this.h.get<LabBooking[]>(`${api}/lab-bookings/me`)} confirm(id:number){return this.h.put(`${api}/lab-bookings/${id}/confirm`,{})} cancel(id:number){return this.h.delete(`${api}/lab-bookings/${id}`)} allBookings(page=1){return this.h.get<LabBooking[]>(`${api}/lab-bookings`,{params:{page}})}
}
