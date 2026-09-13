import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../core/services/portal-services';
import { EventItem, EventRegistration, EventSeat } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ShortTextPipe } from '../../shared/pipes/short-text.pipe';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';
import { HighlightDirective } from '../../shared/directives/highlight.directive';
@Component({selector:'app-events',standalone:true,imports:[CommonModule,FormsModule,StatusBadgeComponent,EmptyStateComponent,ShortTextPipe,HighlightDirective],template:`
<div class="page"><div class="page-head"><div><h1>University events</h1><p>Discover upcoming events, choose reserved seats where required and manage your registrations.</p></div></div>
<div class="toolbar"><div class="field"><label>Show</label><select class="select" [(ngModel)]="filter" (ngModelChange)="loadEvents()"><option value="upcoming">Upcoming events</option><option value="all">All events</option></select></div><div class="field"><label>Search locally</label><input class="input" [(ngModel)]="search" placeholder="Title or venue"></div></div>
<div class="grid grid-3">@for(e of visibleEvents;track e.eventId){<article class="card" appHighlight><div class="row between"><span class="badge primary">{{e.venueName}}</span><span class="small muted">{{e.startAt|date:'MMM d'}}</span></div><h3 style="margin-top:14px">{{e.title}}</h3><p class="muted small">{{e.description|shortText:115}}</p><div class="small"><strong>{{e.startAt|date:'medium'}}</strong><div class="muted">until {{e.endAt|date:'shortTime'}}</div></div><div style="margin-top:15px"><div class="row between small"><span class="muted">Registration</span><strong>{{e.registeredCount}} / {{e.capacity}}</strong></div><div class="progress" style="margin-top:6px"><span [style.width.%]="e.capacity?Math.min(100,e.registeredCount/e.capacity*100):0"></span></div></div><div class="row" style="margin-top:18px"><button class="btn btn-primary grow" [disabled]="e.availableSeats<=0||isRegistered(e.eventId)" (click)="openRegistration(e)">{{isRegistered(e.eventId)?'Already registered':e.availableSeats<=0?'Full':'Register'}}</button></div></article>}@empty{<div class="card" style="grid-column:1/-1"><app-empty-state icon="◇" title="No matching events" message="Try changing the filter or search text."/></div>}</div>
<div class="card" style="margin-top:18px"><div class="row between"><div><h3 class="card-title">My event registrations</h3><div class="card-sub">Held registrations must be confirmed before their hold expires.</div></div><button class="btn btn-secondary btn-sm" (click)="refresh()">Refresh</button></div><div class="divider"></div>@if(registrations.length){<div class="table-wrap"><table class="table"><thead><tr><th>Event</th><th>Starts</th><th>Venue</th><th>Seat</th><th>Status</th><th></th></tr></thead><tbody>@for(r of registrations;track r.eventRegistrationId){<tr><td><strong>{{r.eventTitle}}</strong></td><td>{{r.startAt|date:'medium'}}</td><td>{{r.venueName||'—'}}</td><td>{{r.seatNumber||'General'}}</td><td><app-status-badge [value]="regStatus(r.status)"/></td><td class="right"><div class="row" style="justify-content:flex-end">@if(r.status===1){<button class="btn btn-success btn-sm" (click)="confirmReg(r)">Confirm</button>}@if(r.status===1||r.status===2){<button class="btn btn-danger btn-sm" (click)="cancelReg(r)">Cancel</button>}</div></td></tr>}</tbody></table></div>}@else{<app-empty-state icon="◇" title="No event registrations" message="Register for an event to see it here."/>}</div>
@if(regEvent){<div class="modal-backdrop" (click)="regEvent=null"><div class="modal" (click)="$event.stopPropagation()"><div class="modal-head"><div><h2>Register for {{regEvent.title}}</h2><div class="muted small">{{regEvent.startAt|date:'medium'}} · {{regEvent.venueName}}</div></div><button class="icon-btn" (click)="regEvent=null">×</button></div>@if(regEvent.usesReservedSeating){<p class="muted">This event uses reserved seating. Choose one of the currently available seats.</p>@if(loadingSeats){<div class="empty">Loading seats…</div>}@else if(seats.length){<div class="row wrap">@for(s of seats;track s.eventSeatId){<button class="btn btn-sm" [class.btn-primary]="seatId===s.eventSeatId" [class.btn-secondary]="seatId!==s.eventSeatId" (click)="seatId=s.eventSeatId">{{s.seatNumber}} @if(s.sectionName){· {{s.sectionName}}}</button>}</div>}@else{<div class="alert warning">No seats are currently available.</div>}}@else{<div class="alert info">General admission: no seat selection is required.</div>}<div class="row" style="margin-top:20px"><button class="btn btn-primary" [disabled]="registering||(regEvent.usesReservedSeating&&!seatId)" (click)="register()">{{registering?'Creating hold…':'Create registration hold'}}</button><button class="btn btn-secondary" (click)="regEvent=null">Cancel</button></div></div></div>}
</div>`})
export class EventsComponent implements OnInit{
  events:EventItem[]=[];
  registrations:EventRegistration[]=[];
  filter='all';
  search='';
  regEvent:EventItem|null=null;
  seats:EventSeat[]=[];
  seatId:number|null=null;
  loadingSeats=false;
  registering=false;
  Math=Math;

  constructor(private service:EventService,private errors:ApiErrorService,private toast:ToastService,private cdr:ChangeDetectorRef){}

  ngOnInit(){this.refresh()}
  refresh(){this.loadEvents();this.loadMine()}
  get visibleEvents(){const q=this.search.toLowerCase().trim();return this.events.filter(e=>!q||e.title.toLowerCase().includes(q)||e.venueName.toLowerCase().includes(q))}
  loadEvents(){const filters:any={isPublished:true,pageSize:100};if(this.filter==='upcoming')filters.from=new Date().toISOString();this.service.events(filters).subscribe({next:x=>{this.events=x.filter(e=>e.isActive&&e.isPublished);this.cdr.markForCheck()},error:e=>this.toast.error(this.errors.message(e))})}
  loadMine(){this.service.mine().subscribe({next:x=>{this.registrations=x;this.cdr.markForCheck()},error:e=>this.toast.error(this.errors.message(e))})}
  isRegistered(eventId:number){return this.registrations.some(r=>r.eventId===eventId&&(r.status===1||r.status===2))}
  regStatus(n:number){return ({1:'Held',2:'Confirmed',3:'Cancelled',4:'Expired'} as any)[n]||String(n)}
  openRegistration(e:EventItem){this.regEvent=e;this.seatId=null;this.seats=[];if(e.usesReservedSeating){this.loadingSeats=true;this.service.seatAvailability(e.eventId).subscribe({next:x=>{this.seats=x.filter(s=>s.isAvailable!==false);this.loadingSeats=false},error:er=>{this.loadingSeats=false;this.toast.error(this.errors.message(er))}})}}
  register(){if(!this.regEvent)return;this.registering=true;this.service.register(this.regEvent.eventId,this.seatId).subscribe({next:()=>{this.registering=false;this.regEvent=null;this.toast.success('Registration hold created. Confirm it before it expires.');this.refresh()},error:e=>{this.registering=false;this.toast.error(this.errors.message(e));this.loadEvents()}})}
  confirmReg(r:EventRegistration){this.service.confirm(r.eventRegistrationId).subscribe({next:()=>{this.toast.success('Event registration confirmed.');this.refresh()},error:e=>this.toast.error(this.errors.message(e))})}
  cancelReg(r:EventRegistration){if(!confirm('Cancel this event registration?'))return;this.service.cancel(r.eventRegistrationId).subscribe({next:()=>{this.toast.success('Registration cancelled.');this.refresh()},error:e=>this.toast.error(this.errors.message(e))})}
}
