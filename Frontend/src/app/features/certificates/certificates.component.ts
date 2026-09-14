import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CertificateService } from '../../core/services/portal-services';
import { CertificateRequest, CertificateType } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-certificates',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, StatusBadgeComponent, EmptyStateComponent],
  template: `
    <div class="page">
      <div class="page-head"><div><h1>Certificate requests</h1><p>Request an official university document and track its approval or collection readiness.</p></div><button class="btn btn-primary" (click)="toggleForm()">{{showForm?'Close':'New request'}}</button></div>
      @if(showForm){
        <form class="card stack" [formGroup]="form" (ngSubmit)="submit()" style="margin-bottom:18px"><div><h3 class="card-title">New certificate request</h3><div class="card-sub">Submit one certificate request at a time.</div></div><div class="divider"></div><div class="field"><label for="certificateType">Certificate type</label><select class="select" id="certificateType" formControlName="certificateTypeId"><option [ngValue]="null">Choose document</option>@for(t of types;track t.certificateTypeId){<option [ngValue]="t.certificateTypeId">{{t.name}}</option>}</select></div><div class="field"><label for="certificateReason">Reason</label><textarea class="textarea" id="certificateReason" maxlength="500" formControlName="reason" placeholder="Enter the reason for this request"></textarea></div><div class="row"><button class="btn btn-primary" [disabled]="form.invalid||submitting">{{submitting?'Submitting…':'Submit request'}}</button></div></form>
      }
      <div class="grid grid-3">@for(r of requests;track r.certificateRequestId){<article class="card"><div class="row between"><span class="badge primary">#{{r.certificateRequestId}}</span><app-status-badge [value]="r.status"/></div><h3 style="margin-top:14px">{{r.certificateTypeName}}</h3><div class="small muted">Requested {{r.requestedAt|date:'mediumDate'}}</div>@if(r.reason){<p class="small">{{r.reason}}</p>}@if(r.reviewNote){<div class="alert info"><strong>Review note</strong><div class="small" style="margin-top:5px">{{r.reviewNote}}</div></div>}</article>}@empty{<div class="card" style="grid-column:1/-1"><app-empty-state icon="▤" title="No certificate requests" message="Request a configured official document to see its status here."/></div>}</div>
    </div>`
})
export class CertificatesComponent implements OnInit {
  types: CertificateType[] = [];
  requests: CertificateRequest[] = [];
  showForm = false;
  submitting = false;
  form = new FormGroup({
    certificateTypeId: new FormControl<number | null>(null, { validators: [Validators.required] }),
    reason: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(500)] })
  });

  constructor(private service: CertificateService, private errors: ApiErrorService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.service.types().subscribe({
      next: x => { this.types = x.filter(t => t.isActive); this.cdr.markForCheck(); },
      error: e => { this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
    this.load();
  }

  toggleForm() { this.showForm = !this.showForm; }

  load() {
    this.service.mine().subscribe({
      next: x => { this.requests = x; this.cdr.markForCheck(); },
      error: e => { this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }

  submit() {
    if (this.form.invalid || this.submitting) return;
    this.submitting = true;
    const { certificateTypeId, reason } = this.form.getRawValue();
    this.service.create(certificateTypeId!, reason.trim()).subscribe({
      next: () => {
        this.submitting = false;
        this.showForm = false;
        this.form.reset({ certificateTypeId: null, reason: '' });
        this.toast.success('Certificate request submitted.');
        this.load();
      },
      error: e => { this.submitting = false; this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }
}
