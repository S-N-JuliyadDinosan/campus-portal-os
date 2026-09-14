import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { forkJoin, map, of, switchMap } from 'rxjs';
import { EventItem, EventRegistration, EventSeat, Venue } from '../models/event.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class EventService{
 constructor(private h:HttpClient){}
 events(filters:Record<string,string|number|boolean|undefined>={}){let p=new HttpParams();Object.entries(filters).forEach(([k,v])=>{if(v!==undefined&&v!=='')p=p.set(k,String(v))});return this.h.get<EventItem[]>(`${api}/events`,{params:p})} event(id:number){return this.h.get<EventItem>(`${api}/events/${id}`)} createEvent(b:unknown){return this.h.post<EventItem>(`${api}/events`,b)} updateEvent(id:number,b:unknown){return this.h.put(`${api}/events/${id}`,b)} deleteEvent(id:number){return this.h.delete(`${api}/events/${id}`)}
 venues(){return this.h.get<Venue[]>(`${api}/venues`)} createVenue(b:unknown){return this.h.post<Venue>(`${api}/venues`,b)} updateVenue(id:number,b:unknown){return this.h.put(`${api}/venues/${id}`,b)} deleteVenue(id:number){return this.h.delete(`${api}/venues/${id}`)}
 seats(eventId:number){return this.h.get<EventSeat[]>(`${api}/events/${eventId}/seats`)} seatAvailability(eventId:number){return this.h.get<EventSeat[]>(`${api}/events/${eventId}/seats/availability`)} createSeat(eventId:number,b:unknown){return this.h.post<EventSeat>(`${api}/events/${eventId}/seats`,b)} updateSeat(id:number,b:unknown){return this.h.put(`${api}/event-seats/${id}`,b)} deleteSeat(id:number){return this.h.delete(`${api}/event-seats/${id}`)}
 register(eventId:number,eventSeatId?:number|null){return this.h.post<EventRegistration>(`${api}/event-registrations`,{eventId,eventSeatId:eventSeatId??null})} confirm(id:number){return this.h.put(`${api}/event-registrations/${id}/confirm`,{})} mine(){return this.h.get<EventRegistration[]>(`${api}/event-registrations/me`).pipe(switchMap(rows=>rows.length?forkJoin(rows.map(summary=>this.h.get<EventRegistration>(`${api}/event-registrations/${summary.eventRegistrationId}`).pipe(map(full=>({...full,startAt:summary.startAt,venueName:summary.venueName}))))):of([] as EventRegistration[])))} cancel(id:number){return this.h.delete(`${api}/event-registrations/${id}`)} registrations(filters:Record<string,string|number|undefined>={}){let p=new HttpParams();Object.entries(filters).forEach(([k,v])=>{if(v!==undefined&&v!=='')p=p.set(k,String(v))});return this.h.get<EventRegistration[]>(`${api}/event-registrations`,{params:p})}
}
