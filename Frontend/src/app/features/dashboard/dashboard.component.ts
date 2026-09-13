import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AdminDashboard, StudentDashboard } from '../../core/models/domain.models';
import { ApiErrorService } from '../../core/services/api-error.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/portal-services';
import { AppIconComponent } from '../../shared/components/app-icon.component';
import { LoadingComponent } from '../../shared/components/loading.component';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, StatCardComponent, StatusBadgeComponent, LoadingComponent, AppIconComponent],
  template: `
    <div class="page">
      @if (loading) {
        <app-loading/>
      } @else if (error) {
        <div class="alert error">{{error}}</div>
      } @else if (auth.role()==='Student' && student) {
        <section class="dashboard-hero">
          <div class="hero-content">
            <div class="eyebrow"><app-icon name="sparkles" [size]="15"/> Student workspace</div>
            <h1>{{greeting}}, {{displayName}}.</h1>
            <p>Here is a clear view of your campus services, upcoming reservations, and requests that may need your attention.</p>
            <div class="hero-actions">
              <a class="btn btn-primary" routerLink="/labs">Reserve a lab <app-icon name="arrow-right" [size]="16"/></a>
              <a class="btn btn-secondary" routerLink="/events">Explore events</a>
            </div>
          </div>
          <div class="hero-visual" aria-hidden="true">
            <div class="hero-mini-card"><span>Portal status</span><strong>All services online</strong></div>
            <div class="hero-mini-card"><span>Labs</span><strong>{{student.upcomingLabBookings.length}}</strong></div>
            <div class="hero-mini-card"><span>Events</span><strong>{{student.registeredEvents.length}}</strong></div>
          </div>
        </section>

        <div class="grid grid-4" style="margin-top:18px">
          <app-stat-card icon="building" [value]="student.currentHostelApplication?.status || 'Not applied'" label="Hostel application"/>
          <app-stat-card icon="lab" [value]="student.upcomingLabBookings.length" label="Upcoming lab sessions"/>
          <app-stat-card icon="calendar" [value]="student.registeredEvents.length" label="Registered events"/>
          <app-stat-card icon="bell" [value]="student.unreadNotifications" label="Unread notifications"/>
        </div>

        <div class="grid grid-2" style="margin-top:18px">
          <section class="card">
            <div class="section-head">
              <div><div class="section-kicker">Schedule</div><h3 class="card-title">Upcoming lab bookings</h3><div class="card-sub">Your next reserved lab sessions</div></div>
              <a routerLink="/labs" class="btn btn-ghost btn-sm">View all <app-icon name="arrow-right" [size]="14"/></a>
            </div>
            <div class="divider"></div>
            @if (student.upcomingLabBookings.length) {
              <div class="data-list">
                @for (booking of student.upcomingLabBookings.slice(0,4); track booking.labBookingId) {
                  <div class="data-row">
                    <div class="activity-marker"><app-icon name="lab" [size]="17"/></div>
                    <div class="grow"><strong>{{booking.labName}}</strong><div class="small muted">{{booking.bookingDate|date:'mediumDate'}} · {{booking.startTime}}–{{booking.endTime}}</div></div>
                    <span class="badge info">Booked</span>
                  </div>
                }
              </div>
            } @else {
              <div class="empty"><div class="icon"><app-icon name="lab"/></div><strong>No upcoming lab bookings</strong><div class="small" style="margin-top:6px">Reserve a lab when you need one.</div></div>
            }
          </section>

          <section class="card">
            <div class="section-head">
              <div><div class="section-kicker">Campus life</div><h3 class="card-title">Registered events</h3><div class="card-sub">Events already on your calendar</div></div>
              <a routerLink="/events" class="btn btn-ghost btn-sm">Explore <app-icon name="arrow-right" [size]="14"/></a>
            </div>
            <div class="divider"></div>
            @if (student.registeredEvents.length) {
              <div class="data-list">
                @for (event of student.registeredEvents.slice(0,4); track event.eventRegistrationId) {
                  <div class="data-row">
                    <div class="event-date">{{event.startAt|date:'d'}}<span>{{event.startAt|date:'MMM'}}</span></div>
                    <div class="grow"><strong>{{event.title}}</strong><div class="small muted">{{event.startAt|date:'shortTime'}} · {{event.venue}}</div></div>
                    <span class="badge primary">Registered</span>
                  </div>
                }
              </div>
            } @else {
              <div class="empty"><div class="icon"><app-icon name="calendar"/></div><strong>No registered events</strong><div class="small" style="margin-top:6px">Explore what is happening around campus.</div></div>
            }
          </section>
        </div>

        <section class="card" style="margin-top:18px">
          <div class="section-head">
            <div><div class="section-kicker">Documents</div><h3 class="card-title">Certificate requests</h3><div class="card-sub">Track official document progress</div></div>
            <a routerLink="/certificates" class="btn btn-ghost btn-sm">Manage <app-icon name="arrow-right" [size]="14"/></a>
          </div>
          <div class="divider"></div>
          @if (student.certificateRequests.length) {
            <div class="grid grid-3">
              @for (request of student.certificateRequests.slice(0,6); track request.certificateRequestId) {
                <div class="card flat event-card">
                  <div class="row between">
                    <div class="activity-marker"><app-icon name="certificate" [size]="17"/></div>
                    <app-status-badge [value]="request.status"/>
                  </div>
                  <strong style="display:block;margin-top:13px">{{request.certificateType}}</strong>
                  <div class="small muted" style="margin-top:5px">Requested {{request.requestedAt|date:'mediumDate'}}</div>
                </div>
              }
            </div>
          } @else {
            <div class="empty"><div class="icon"><app-icon name="certificate"/></div><strong>No certificate requests</strong><div class="small" style="margin-top:6px">New requests will appear here.</div></div>
          }
        </section>
      } @else if (admin) {
        <section class="dashboard-hero">
          <div class="hero-content">
            <div class="eyebrow"><app-icon name="shield" [size]="15"/> Administration center</div>
            <h1>Campus operations, at a glance.</h1>
            <p>Monitor registrations, service queues, complaints, certificates, and fee collection from one operational workspace.</p>
          </div>
          <div class="hero-visual" aria-hidden="true">
            <div class="hero-mini-card"><span>Operational status</span><strong>Systems ready</strong></div>
            <div class="hero-mini-card"><span>Students</span><strong>{{admin.totalRegisteredStudents}}</strong></div>
            <div class="hero-mini-card"><span>Open queues</span><strong>{{admin.pendingHostelApplications + admin.pendingCertificateRequests}}</strong></div>
          </div>
        </section>

        <div class="grid grid-4" style="margin-top:18px">
          <app-stat-card icon="users" [value]="admin.totalRegisteredStudents" label="Registered students"/>
          <app-stat-card icon="building" [value]="admin.pendingHostelApplications" label="Pending hostel requests"/>
          <app-stat-card icon="certificate" [value]="admin.pendingCertificateRequests" label="Pending certificates"/>
          <app-stat-card icon="wallet" [value]="(admin.feeCollection.paidAmount | number:'1.0-0') || '0'" label="LKR fees collected"/>
        </div>

        <div class="grid grid-2" style="margin-top:18px">
          <section class="card">
            <div class="section-head">
              <div><div class="section-kicker">Revenue overview</div><h3 class="card-title">Fee collection</h3><div class="card-sub">Paid versus outstanding balances</div></div>
              <div class="activity-marker"><app-icon name="trending" [size]="18"/></div>
            </div>
            <div class="grid grid-2" style="margin-top:23px">
              <div><div class="muted small">Collected</div><h2 style="margin:6px 0;font-size:1.45rem">LKR {{admin.feeCollection.paidAmount|number:'1.0-0'}}</h2><div class="small muted">{{admin.feeCollection.paidCount}} payments</div></div>
              <div><div class="muted small">Outstanding</div><h2 style="margin:6px 0;font-size:1.45rem">LKR {{admin.feeCollection.outstandingAmount|number:'1.0-0'}}</h2><div class="small muted">{{admin.feeCollection.outstandingCount}} items</div></div>
            </div>
            <div style="margin-top:22px">
              <div class="row between small" style="margin-bottom:7px"><span class="muted">Collection progress</span><strong>{{feePercent|number:'1.0-0'}}%</strong></div>
              <div class="progress"><span [style.width.%]="feePercent"></span></div>
            </div>
          </section>

          <section class="card">
            <div class="section-head">
              <div><div class="section-kicker">Service desk</div><h3 class="card-title">Open complaints</h3><div class="card-sub">Cases by category and workflow stage</div></div>
              <a routerLink="/admin/complaints" class="btn btn-ghost btn-sm">Review <app-icon name="arrow-right" [size]="14"/></a>
            </div>
            <div class="divider"></div>
            @if (admin.openComplaints.length) {
              <div class="data-list">
                @for (complaint of admin.openComplaints; track complaint.category + complaint.status) {
                  <div class="data-row">
                    <div class="activity-marker"><app-icon name="message" [size]="17"/></div>
                    <div class="grow"><strong>{{complaint.category}}</strong><div class="small muted">{{complaint.status}}</div></div>
                    <span class="badge warning">{{complaint.count}} open</span>
                  </div>
                }
              </div>
            } @else {
              <div class="empty"><div class="icon"><app-icon name="check"/></div><strong>Complaint queue is clear</strong><div class="small" style="margin-top:6px">There are no open complaints.</div></div>
            }
          </section>
        </div>

        <section class="card" style="margin-top:18px">
          <div class="section-head">
            <div><div class="section-kicker">Upcoming</div><h3 class="card-title">Event capacity</h3><div class="card-sub">Registration load across scheduled events</div></div>
            <a routerLink="/admin/events" class="btn btn-ghost btn-sm">Manage events <app-icon name="arrow-right" [size]="14"/></a>
          </div>
          <div class="divider"></div>
          @if (admin.upcomingEvents.length) {
            <div class="grid grid-3">
              @for (event of admin.upcomingEvents; track event.eventId) {
                <div class="card flat event-card">
                  <div class="row">
                    <div class="event-date">{{event.startAt|date:'d'}}<span>{{event.startAt|date:'MMM'}}</span></div>
                    <div class="grow"><strong>{{event.title}}</strong><div class="small muted">{{event.startAt|date:'shortTime'}}</div></div>
                  </div>
                  <div class="row between small" style="margin:17px 0 7px"><span class="muted">Registrations</span><strong>{{event.registrationCount}} / {{event.capacity}}</strong></div>
                  <div class="progress"><span [style.width.%]="event.capacity ? Math.min(100,event.registrationCount/event.capacity*100) : 0"></span></div>
                </div>
              }
            </div>
          } @else {
            <div class="empty"><div class="icon"><app-icon name="calendar"/></div><strong>No upcoming events</strong><div class="small" style="margin-top:6px">Scheduled events will appear here.</div></div>
          }
        </section>
      }
    </div>
  `
})
export class DashboardComponent implements OnInit {
  loading = true;
  error = '';
  student: StudentDashboard | null = null;
  admin: AdminDashboard | null = null;
  readonly Math = Math;

  get greeting(): string {
    const hour = new Date().getHours();
    return hour < 12 ? 'Good morning' : hour < 18 ? 'Good afternoon' : 'Good evening';
  }

  get displayName(): string {
    const name = this.auth.user().email?.split('@')[0]?.replace(/[._-]+/g, ' ') || 'student';
    return name.replace(/\b\w/g, char => char.toUpperCase());
  }

  get feePercent(): number {
    if (!this.admin) return 0;
    const total = this.admin.feeCollection.paidAmount + this.admin.feeCollection.outstandingAmount;
    return total ? this.admin.feeCollection.paidAmount / total * 100 : 0;
  }

  constructor(
    public auth: AuthService,
    private service: DashboardService,
    private errors: ApiErrorService
  ) {}

  ngOnInit(): void {
    if (this.auth.role() === 'Admin') {
      this.service.admin().subscribe({
        next: data => { this.admin = data; this.loading = false; },
        error: (error: unknown) => { this.error = this.errors.message(error); this.loading = false; }
      });
    } else {
      this.service.student().subscribe({
        next: data => { this.student = data; this.loading = false; },
        error: (error: unknown) => { this.error = this.errors.message(error); this.loading = false; }
      });
    }
  }
}
