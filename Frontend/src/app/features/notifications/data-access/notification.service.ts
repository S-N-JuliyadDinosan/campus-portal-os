import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { NotificationItem } from '../models/notification.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class NotificationService{
 constructor(private h:HttpClient){} unread=signal(0); list(isRead?:boolean,page=1){let p=new HttpParams().set('page',page);if(isRead!==undefined)p=p.set('isRead',isRead);return this.h.get<NotificationItem[]>(`${api}/notifications/me`,{params:p})} refreshUnread(){this.h.get<number>(`${api}/notifications/me/unread-count`).subscribe({next:n=>this.unread.set(n),error:()=>this.unread.set(0)})} mark(id:number){return this.h.put(`${api}/notifications/${id}/read`,{})} markAll(){return this.h.put(`${api}/notifications/me/read-all`,{})}}
