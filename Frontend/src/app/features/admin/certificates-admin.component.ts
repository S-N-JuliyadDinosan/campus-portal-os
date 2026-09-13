import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CertificateService } from '../../core/services/portal-services';
import { CertificateRequest, CertificateType } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-certificates-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, StatusBadgeComponent],
  template: `
    <div class="page">
      <div class="page-head"><div><h1>Certificate administration</h1><p>Review document requests, update collection workflow and maintain certificate types.</p></div></div>
      <div class="tabs"><button class="tab" [class.active]="tab==='requests'" (click)="tab='requests';loadRequests()">Requests</button><button class="tab" [class.active]="tab==='types'" (click)="tab='types';loadTypes()">Certificate types</button></div>

      @if (tab === 'requests') {
        <div class="card">
          <div class="toolbar"><div class="field"><label>Status</label><select class="select" [(ngModel)]="statusFilter"><option value="">All</option>@for (status of statuses; track status) {<option [value]="status">{{label(status)}}</option>}</select></div><button class="btn btn-primary" (click)="loadRequests()">Filter</button></div>
          <div class="table-wrap"><table class="table"><thead><tr><th>ID</th><th>Student</th><th>Certificate</th><th>Reason</th><th>Status</th><th>Requested</th><th></th></tr></thead><tbody>
            @for (request of requests; track request.certificateRequestId) {
              <tr><td>{{request.certificateRequestId}}</td><td><strong>{{request.studentName}}</strong><div class="small muted">{{request.studentIndexNumber || request.studentId}}</div></td><td><strong>{{request.certificateTypeName}}</strong></td><td>{{request.reason || '—'}}</td><td><app-status-badge [value]="request.status"/></td><td>{{request.requestedAt|date:'mediumDate'}}</td><td class="right"><button class="btn btn-secondary btn-sm" (click)="editRequest(request)">Review</button></td></tr>
            }
          </tbody></table></div>
        </div>
      }

      @if (tab === 'types') {
        <div class="grid grid-3">
          <div class="card">
            <h3 class="card-title">{{typeEditId ? 'Edit' : 'Create'}} certificate type</h3>
            <form class="stack" #typeFormRef="ngForm" (ngSubmit)="saveType()" style="margin-top:16px">
              <div class="field"><label>Name</label><input class="input" required maxlength="150" name="name" [(ngModel)]="typeForm.name"></div>
              <div class="field"><label>Description</label><textarea class="textarea" maxlength="500" name="desc" [(ngModel)]="typeForm.description"></textarea></div>
              @if (typeEditId) {<label class="row"><input type="checkbox" name="active" [(ngModel)]="typeForm.isActive"> Active</label>}
              <div class="row"><button class="btn btn-primary" [disabled]="typeFormRef.invalid">Save</button>@if (typeEditId) {<button type="button" class="btn btn-secondary" (click)="resetType()">Cancel</button>}</div>
            </form>
          </div>
          <div class="card" style="grid-column:span 2">
            <h3 class="card-title">Certificate types</h3><div class="divider"></div>
            <div class="table-wrap"><table class="table"><thead><tr><th>Name</th><th>Description</th><th>Status</th><th></th></tr></thead><tbody>
              @for (type of types; track type.certificateTypeId) {<tr><td><strong>{{type.name}}</strong></td><td>{{type.description || '—'}}</td><td><app-status-badge [value]="type.isActive ? 'Active' : 'Inactive'"/></td><td class="right"><div class="row" style="justify-content:flex-end"><button class="btn btn-secondary btn-sm" (click)="editType(type)">Edit</button><button class="btn btn-danger btn-sm" (click)="deleteType(type)">Delete</button></div></td></tr>}
            </tbody></table></div>
          </div>
        </div>
      }

      @if (selected) {
        <div class="modal-backdrop" (click)="selected=null"><div class="modal" (click)="$event.stopPropagation()">
          <div class="modal-head"><div><h2>Review request {{selected.certificateRequestId}}</h2><div class="small muted">{{selected.studentName}} · {{selected.certificateTypeName}}</div></div><button class="icon-btn" (click)="selected=null">×</button></div>
          <form class="stack" #requestForm="ngForm" (ngSubmit)="saveRequest()"><div class="field"><label>Status</label><select class="select" required name="status" [(ngModel)]="requestStatus">@for (status of statuses; track status) {<option [value]="status">{{label(status)}}</option>}</select></div><div class="field"><label>Review note</label><textarea class="textarea" name="note" [(ngModel)]="reviewNote"></textarea></div><div class="row"><button class="btn btn-primary" [disabled]="requestForm.invalid">Update request</button><button type="button" class="btn btn-secondary" (click)="selected=null">Cancel</button></div></form>
        </div></div>
      }
    </div>
  `
})
export class CertificatesAdminComponent implements OnInit {
  tab: 'requests' | 'types' = 'requests';
  statuses = ['Pending', 'Approved', 'Rejected', 'ReadyForCollection', 'Collected'];
  requests: CertificateRequest[] = [];
  types: CertificateType[] = [];
  statusFilter = '';
  selected: CertificateRequest | null = null;
  requestStatus = 'Pending';
  reviewNote = '';
  typeEditId: number | null = null;
  typeForm: { name: string; description: string; isActive: boolean } = { name: '', description: '', isActive: true };

  constructor(private service: CertificateService, private errors: ApiErrorService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.loadTypes(); this.loadRequests(); }
  label(status: string) { return status.replace(/([a-z])([A-Z])/g, '$1 $2'); }
  loadRequests() { this.service.all(this.statusFilter || undefined).subscribe({ next: requests => { this.requests = requests; this.cdr.markForCheck(); }, error: error => { this.toast.error(this.errors.message(error)); this.cdr.markForCheck(); } }); }
  editRequest(request: CertificateRequest) { this.selected = request; this.requestStatus = request.status; this.reviewNote = request.reviewNote || ''; }
  saveRequest() { if (!this.selected) return; this.service.updateStatus(this.selected.certificateRequestId, this.requestStatus, this.reviewNote || undefined).subscribe({ next: () => { this.selected = null; this.toast.success('Certificate request updated.'); this.loadRequests(); }, error: error => this.toast.error(this.errors.message(error)) }); }
  loadTypes() { this.service.types().subscribe({ next: types => { this.types = types; this.cdr.markForCheck(); }, error: error => { this.toast.error(this.errors.message(error)); this.cdr.markForCheck(); } }); }
  editType(type: CertificateType) { this.typeEditId = type.certificateTypeId; this.typeForm = { name: type.name, description: type.description || '', isActive: type.isActive }; }
  resetType() { this.typeEditId = null; this.typeForm = { name: '', description: '', isActive: true }; }
  saveType() { const request = this.typeEditId ? this.service.updateType(this.typeEditId, this.typeForm) : this.service.createType({ name: this.typeForm.name, description: this.typeForm.description || null }); request.subscribe({ next: () => { this.toast.success('Certificate type saved.'); this.resetType(); this.loadTypes(); }, error: error => this.toast.error(this.errors.message(error)) }); }
  deleteType(type: CertificateType) { if (!confirm(`Delete ${type.name}?`)) return; this.service.deleteType(type.certificateTypeId).subscribe({ next: () => { this.toast.success('Certificate type deleted.'); this.loadTypes(); }, error: error => this.toast.error(this.errors.message(error)) }); }
}
