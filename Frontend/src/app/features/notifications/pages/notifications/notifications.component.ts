import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../data-access/notification.service';
import { NotificationItem } from '../../models/notification.models';
import { EmptyStateComponent } from '../../../../shared/components/empty-state.component';
import { ApiErrorService } from '../../../../core/http/api-error.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule, EmptyStateComponent],
  template: `
    <div class="page"><div class="page-head"><div><h1>Notifications</h1><p>Status changes and system messages relevant to your own campus activity.</p></div><button class="btn btn-secondary" [disabled]="!service.unread()" (click)="markAll()">Mark all as read</button></div><div class="tabs"><button class="tab" [class.active]="filter==='all'" (click)="setFilter('all')">All</button><button class="tab" [class.active]="filter==='unread'" (click)="setFilter('unread')">Unread</button><button class="tab" [class.active]="filter==='read'" (click)="setFilter('read')">Read</button></div>@if(items.length){<div class="stack">@for(n of items;track n.notificationId){<article class="card" [style.opacity]="n.isRead?'.72':'1'" [style.border-left]="n.isRead?'1px solid #e2e8f0':'4px solid #6366f1'"><div class="row between"><div class="row"><span class="stat-icon" style="width:38px;height:38px">{{icon(n.type)}}</span><div><strong>{{n.title}}</strong><div class="small muted">{{n.createdAt|date:'medium'}}</div></div></div>@if(!n.isRead){<span class="badge primary">New</span>}</div><p style="margin-bottom:0">{{n.message}}</p>@if(!n.isRead){<div class="row" style="margin-top:12px"><button class="btn btn-secondary btn-sm" (click)="mark(n)">Mark as read</button></div>}</article>}</div>}@else{<div class="card"><app-empty-state icon="◌" title="No notifications" message="You're all caught up for this filter."/></div>}</div>`
})
export class NotificationsComponent implements OnInit {
  items: NotificationItem[] = [];
  filter: 'all' | 'unread' | 'read' = 'all';

  constructor(public service: NotificationService, private errors: ApiErrorService, private toast: ToastService, private cdr: ChangeDetectorRef) {}
  ngOnInit() { this.load(); }

  setFilter(f: 'all' | 'unread' | 'read') { this.filter = f; this.load(); }

  load() {
    const read = this.filter === 'all' ? undefined : this.filter === 'read';
    this.service.list(read).subscribe({
      next: x => { this.items = x; this.cdr.markForCheck(); },
      error: e => { this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }

  mark(n: NotificationItem) {
    this.service.mark(n.notificationId).subscribe({
      next: () => {
        n.isRead = true;
        this.service.refreshUnread();
        if (this.filter === 'unread') this.items = this.items.filter(x => x.notificationId !== n.notificationId);
        this.cdr.markForCheck();
      },
      error: e => this.toast.error(this.errors.message(e))
    });
  }

  markAll() {
    this.service.markAll().subscribe({
      next: () => { this.toast.success('All notifications marked as read.'); this.service.unread.set(0); this.load(); },
      error: e => this.toast.error(this.errors.message(e))
    });
  }

  icon(type: string) {
    const t = type.toLowerCase();
    return t.includes('hostel') ? '⌂' : t.includes('event') ? '◇' : t.includes('complaint') ? '!' : t.includes('certificate') ? '▤' : t.includes('fee') ? 'LKR' : '•';
  }
}
