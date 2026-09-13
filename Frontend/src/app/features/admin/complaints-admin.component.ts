import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplaintService } from '../../core/services/portal-services';
import { Complaint, ComplaintCategory } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-complaints-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, StatusBadgeComponent],
  template: `
    <div class="page">
      <div class="page-head"><div><h1>Complaint administration</h1><p>Triage complaints, update workflow status and maintain complaint categories.</p></div></div>
      <div class="tabs">
        <button class="tab" [class.active]="tab==='complaints'" (click)="tab='complaints';loadComplaints()">Complaints</button>
        <button class="tab" [class.active]="tab==='categories'" (click)="tab='categories';loadCategories()">Categories</button>
      </div>
      @if(tab==='complaints'){
        <div class="card"><div class="toolbar"><div class="field"><label>Status</label><select class="select" [(ngModel)]="statusFilter"><option value="">All</option><option [ngValue]="1">Pending</option><option [ngValue]="2">In Progress</option><option [ngValue]="3">Resolved</option></select></div><button class="btn btn-primary" (click)="loadComplaints()">Filter</button></div><div class="table-wrap"><table class="table"><thead><tr><th>ID</th><th>Student</th><th>Category</th><th>Description</th><th>Status</th><th>Resolution</th><th></th></tr></thead><tbody>@for(c of complaints;track c.complaintId){<tr><td>#{{c.complaintId}}</td><td>#{{c.studentId}}</td><td>{{c.categoryName}}</td><td style="max-width:300px">{{c.description}}</td><td><app-status-badge [value]="status(c.status)"/></td><td>{{c.resolutionNote||'—'}}</td><td class="right"><button class="btn btn-secondary btn-sm" (click)="editComplaint(c)">Update</button></td></tr>}</tbody></table></div></div>
      }
      @if(tab==='categories'){
        <div class="grid grid-3"><div class="card"><h3 class="card-title">{{categoryEditId?'Edit':'Create'}} category</h3><form class="stack" #cf="ngForm" (ngSubmit)="saveCategory()" style="margin-top:16px"><div class="field"><label>Name</label><input class="input" required name="name" [(ngModel)]="categoryForm.name"></div><div class="field"><label>Description</label><textarea class="textarea" name="desc" [(ngModel)]="categoryForm.description"></textarea></div>@if(categoryEditId){<label class="row"><input type="checkbox" name="active" [(ngModel)]="categoryForm.isActive"> Active</label>}<div class="row"><button type="submit" class="btn btn-primary" [disabled]="cf.invalid||savingCategory">{{savingCategory?'Saving…':'Save'}}</button>@if(categoryEditId){<button type="button" class="btn btn-secondary" [disabled]="savingCategory" (click)="resetCategory()">Cancel</button>}</div></form></div><div class="card" style="grid-column:span 2"><h3 class="card-title">Complaint categories</h3><div class="divider"></div><div class="table-wrap"><table class="table"><thead><tr><th>Name</th><th>Description</th><th>Status</th><th></th></tr></thead><tbody>@for(c of categories;track c.complaintCategoryId){<tr><td><strong>{{c.name}}</strong></td><td>{{c.description||'—'}}</td><td><app-status-badge [value]="c.isActive?'Active':'Inactive'"/></td><td class="right"><div class="row" style="justify-content:flex-end"><button class="btn btn-secondary btn-sm" (click)="editCategory(c)">Edit</button><button class="btn btn-danger btn-sm" (click)="deleteCategory(c)">Delete</button></div></td></tr>}</tbody></table></div></div></div>
      }
      @if(selected){
        <div class="modal-backdrop" (click)="selected=null"><div class="modal" (click)="$event.stopPropagation()"><div class="modal-head"><div><h2>Update complaint #{{selected.complaintId}}</h2><div class="small muted">{{selected.categoryName}}</div></div><button class="icon-btn" (click)="selected=null">×</button></div><div class="alert info">{{selected.description}}</div><form class="stack" #uf="ngForm" (ngSubmit)="saveStatus()" style="margin-top:16px"><div class="field"><label>Status</label><select class="select" required name="status" [(ngModel)]="updateStatus"><option [ngValue]="1">Pending</option><option [ngValue]="2">In Progress</option><option [ngValue]="3">Resolved</option></select></div><div class="field"><label>Resolution note</label><textarea class="textarea" name="note" [(ngModel)]="resolutionNote" [required]="updateStatus===3" placeholder="Required when resolving"></textarea></div><div class="row"><button class="btn btn-primary" [disabled]="uf.invalid">Save status</button><button type="button" class="btn btn-secondary" (click)="selected=null">Cancel</button></div></form></div></div>
      }
    </div>`
})
export class ComplaintsAdminComponent implements OnInit {
  tab: 'complaints' | 'categories' = 'complaints';
  complaints: Complaint[] = [];
  categories: ComplaintCategory[] = [];
  statusFilter: any = '';
  selected: Complaint | null = null;
  updateStatus = 1;
  resolutionNote = '';
  categoryEditId: number | null = null;
  categoryForm: { name: string; description: string; isActive: boolean } = { name: '', description: '', isActive: true };
  savingCategory = false;

  constructor(private service: ComplaintService, private errors: ApiErrorService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.loadCategories(); this.loadComplaints(); }

  loadComplaints() {
    this.service.all(this.statusFilter ? { status: this.statusFilter } : {}).subscribe({
      next: x => { this.complaints = x; this.cdr.markForCheck(); },
      error: e => { this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }

  status(n: number) { return ({ 1: 'Pending', 2: 'In Progress', 3: 'Resolved' } as any)[n] || n; }
  editComplaint(c: Complaint) { this.selected = c; this.updateStatus = c.status; this.resolutionNote = c.resolutionNote || ''; }

  saveStatus() {
    if (!this.selected) return;
    this.service.updateStatus(this.selected.complaintId, this.updateStatus, this.resolutionNote || undefined).subscribe({
      next: () => { this.selected = null; this.toast.success('Complaint status updated.'); this.loadComplaints(); },
      error: e => this.toast.error(this.errors.message(e))
    });
  }

  loadCategories() {
    this.service.categories().subscribe({
      next: x => { this.categories = x; this.cdr.markForCheck(); },
      error: e => { this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }

  editCategory(c: ComplaintCategory) {
    this.categoryEditId = c.complaintCategoryId;
    this.categoryForm = { name: c.name, description: c.description || '', isActive: c.isActive };
  }

  resetCategory() {
    this.categoryEditId = null;
    this.categoryForm = { name: '', description: '', isActive: true };
  }

  saveCategory() {
    if (this.savingCategory) return;
    const name = this.categoryForm.name.trim();
    if (!name) return;

    this.savingCategory = true;
    const payload = { name, description: this.categoryForm.description.trim() || null, isActive: this.categoryForm.isActive };
    const editId = this.categoryEditId;
    const request = editId === null
      ? this.service.createCategory(payload)
      : this.service.updateCategory(editId, payload);

    request.subscribe({
      next: category => {
        if (category?.complaintCategoryId) {
          this.upsertCategory(category);
        } else if (editId !== null) {
          this.upsertCategory({ complaintCategoryId: editId, ...payload });
        } else {
          this.loadCategories();
        }
        this.resetCategory();
        this.savingCategory = false;
        this.toast.success('Category saved.');
        this.cdr.markForCheck();
        this.loadCategories();
      },
      error: e => { this.savingCategory = false; this.toast.error(this.errors.message(e)); this.cdr.markForCheck(); }
    });
  }

  deleteCategory(c: ComplaintCategory) {
    if (!confirm(`Permanently delete ${c.name}? This cannot be undone.`)) return;
    this.service.deleteCategory(c.complaintCategoryId).subscribe({
      next: () => { this.categories = this.categories.filter(x => x.complaintCategoryId !== c.complaintCategoryId); this.toast.success('Category deleted.'); this.cdr.markForCheck(); },
      error: e => this.toast.error(this.errors.message(e))
    });
  }

  private upsertCategory(category: ComplaintCategory) {
    const index = this.categories.findIndex(x => x.complaintCategoryId === category.complaintCategoryId);
    const next = [...this.categories];
    if (index < 0) next.push(category); else next[index] = category;
    this.categories = next.sort((a, b) => a.name.localeCompare(b.name));
  }
}
