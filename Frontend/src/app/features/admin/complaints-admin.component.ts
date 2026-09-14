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
      <div class="page-head">
        <div>
          <h1>Complaint administration</h1>
          <p>Triage complaints, update workflow status and maintain complaint categories.</p>
        </div>
      </div>

      <div class="tabs">
        <button class="tab" [class.active]="tab === 'complaints'" (click)="tab = 'complaints'; loadComplaints()">Complaints</button>
        <button class="tab" [class.active]="tab === 'categories'" (click)="tab = 'categories'; loadCategories()">Categories</button>
      </div>

      @if (tab === 'complaints') {
        <div class="card">
          <div class="toolbar">
            <div class="field">
              <label>Status</label>
              <select class="select" [(ngModel)]="statusFilter">
                <option value="">All</option>
                <option [ngValue]="1">Pending</option>
                <option [ngValue]="2">In Progress</option>
                <option [ngValue]="3">Resolved</option>
              </select>
            </div>
            <button class="btn btn-primary" (click)="loadComplaints()">Filter</button>
          </div>
          <div class="table-wrap">
            <table class="table">
              <thead><tr><th>Complaint ID</th><th>Student</th><th>Category</th><th>Description</th><th>Status</th><th>Resolution</th><th></th></tr></thead>
              <tbody>
                @for (complaint of complaints; track complaint.complaintId) {
                  <tr>
                    <td>{{ complaint.complaintId }}</td>
                    <td>
                      @if (complaint.studentName) {
                        <strong>{{ complaint.studentName }}</strong>
                        <div class="small muted">{{ complaint.studentIndexNumber || complaint.studentId }}</div>
                      } @else {
                        <span>Anonymous</span>
                      }
                    </td>
                    <td>{{ complaint.categoryName }}</td>
                    <td style="max-width:300px">{{ complaint.description }}</td>
                    <td><app-status-badge [value]="status(complaint.status)" /></td>
                    <td>{{ complaint.resolutionNote || '-' }}</td>
                    <td class="right"><button class="btn btn-secondary btn-sm" (click)="editComplaint(complaint)">Update</button></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      @if (tab === 'categories') {
        <div class="grid grid-3">
          <div class="card">
            <h3 class="card-title">{{ categoryEditId ? 'Edit' : 'Create' }} category</h3>
            <form class="stack" #categoryFormElement="ngForm" (ngSubmit)="saveCategory()" style="margin-top:16px">
              <div class="field"><label>Name</label><input class="input" required name="name" [(ngModel)]="categoryForm.name"></div>
              <div class="field"><label>Description</label><textarea class="textarea" name="desc" [(ngModel)]="categoryForm.description"></textarea></div>
              @if (categoryEditId) {<label class="row"><input type="checkbox" name="active" [(ngModel)]="categoryForm.isActive"> Active</label>}
              <div class="row">
                <button type="submit" class="btn btn-primary" [disabled]="categoryFormElement.invalid || savingCategory">{{ savingCategory ? 'Saving...' : 'Save' }}</button>
                @if (categoryEditId) {<button type="button" class="btn btn-secondary" [disabled]="savingCategory" (click)="resetCategory()">Cancel</button>}
              </div>
            </form>
          </div>
          <div class="card" style="grid-column:span 2">
            <h3 class="card-title">Complaint categories</h3>
            <div class="divider"></div>
            <div class="table-wrap">
              <table class="table">
                <thead><tr><th>Name</th><th>Description</th><th>Status</th><th></th></tr></thead>
                <tbody>
                  @for (category of categories; track category.complaintCategoryId) {
                    <tr>
                      <td><strong>{{ category.name }}</strong></td>
                      <td>{{ category.description || '-' }}</td>
                      <td><app-status-badge [value]="category.isActive ? 'Active' : 'Inactive'" /></td>
                      <td class="right"><div class="row" style="justify-content:flex-end"><button class="btn btn-secondary btn-sm" (click)="editCategory(category)">Edit</button><button class="btn btn-danger btn-sm" (click)="deleteCategory(category)">Delete</button></div></td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>
      }

      @if (selected) {
        <div class="modal-backdrop" (click)="selected = null">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-head"><div><h2>Update complaint {{ selected.complaintId }}</h2><div class="small muted">{{ selected.categoryName }}</div></div><button class="icon-btn" (click)="selected = null">x</button></div>
            <div class="alert info">{{ selected.description }}</div>
            <form class="stack" #updateForm="ngForm" (ngSubmit)="saveStatus()" style="margin-top:16px">
              <div class="field"><label>Status</label><select class="select" required name="status" [(ngModel)]="updateStatus"><option [ngValue]="1">Pending</option><option [ngValue]="2">In Progress</option><option [ngValue]="3">Resolved</option></select></div>
              <div class="field"><label>Resolution note</label><textarea class="textarea" name="note" [(ngModel)]="resolutionNote" [required]="updateStatus === 3" placeholder="Required when resolving"></textarea></div>
              <div class="row"><button class="btn btn-primary" [disabled]="updateForm.invalid">Save status</button><button type="button" class="btn btn-secondary" (click)="selected = null">Cancel</button></div>
            </form>
          </div>
        </div>
      }
    </div>`
})
export class ComplaintsAdminComponent implements OnInit {
  tab: 'complaints' | 'categories' = 'complaints';
  complaints: Complaint[] = [];
  categories: ComplaintCategory[] = [];
  statusFilter: number | '' = '';
  selected: Complaint | null = null;
  updateStatus = 1;
  resolutionNote = '';
  categoryEditId: number | null = null;
  categoryForm = { name: '', description: '', isActive: true };
  savingCategory = false;

  constructor(private service: ComplaintService, private errors: ApiErrorService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.loadCategories(); this.loadComplaints(); }

  loadComplaints() {
    this.service.all(this.statusFilter ? { status: this.statusFilter } : {}).subscribe({
      next: complaints => { this.complaints = complaints; this.cdr.markForCheck(); },
      error: error => { this.toast.error(this.errors.message(error)); this.cdr.markForCheck(); }
    });
  }

  status(value: number) { return ({ 1: 'Pending', 2: 'In Progress', 3: 'Resolved' } as Record<number, string>)[value] || value; }
  editComplaint(complaint: Complaint) { this.selected = complaint; this.updateStatus = complaint.status; this.resolutionNote = complaint.resolutionNote || ''; }

  saveStatus() {
    if (!this.selected) return;
    this.service.updateStatus(this.selected.complaintId, this.updateStatus, this.resolutionNote || undefined).subscribe({
      next: () => { this.selected = null; this.toast.success('Complaint status updated.'); this.loadComplaints(); },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  loadCategories() {
    this.service.categories().subscribe({
      next: categories => { this.categories = categories; this.cdr.markForCheck(); },
      error: error => { this.toast.error(this.errors.message(error)); this.cdr.markForCheck(); }
    });
  }

  editCategory(category: ComplaintCategory) {
    this.categoryEditId = category.complaintCategoryId;
    this.categoryForm = { name: category.name, description: category.description || '', isActive: category.isActive };
  }

  resetCategory() { this.categoryEditId = null; this.categoryForm = { name: '', description: '', isActive: true }; }

  saveCategory() {
    if (this.savingCategory) return;
    const name = this.categoryForm.name.trim();
    if (!name) return;
    this.savingCategory = true;
    const payload = { name, description: this.categoryForm.description.trim() || null, isActive: this.categoryForm.isActive };
    const editId = this.categoryEditId;
    const request = editId === null ? this.service.createCategory(payload) : this.service.updateCategory(editId, payload);
    request.subscribe({
      next: category => {
        if (category?.complaintCategoryId) this.upsertCategory(category);
        else if (editId !== null) this.upsertCategory({ complaintCategoryId: editId, ...payload });
        else this.loadCategories();
        this.resetCategory();
        this.savingCategory = false;
        this.toast.success('Category saved.');
        this.cdr.markForCheck();
        this.loadCategories();
      },
      error: error => { this.savingCategory = false; this.toast.error(this.errors.message(error)); this.cdr.markForCheck(); }
    });
  }

  deleteCategory(category: ComplaintCategory) {
    if (!confirm(`Permanently delete ${category.name}? This cannot be undone.`)) return;
    this.service.deleteCategory(category.complaintCategoryId).subscribe({
      next: () => { this.categories = this.categories.filter(item => item.complaintCategoryId !== category.complaintCategoryId); this.toast.success('Category deleted.'); this.cdr.markForCheck(); },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  private upsertCategory(category: ComplaintCategory) {
    const index = this.categories.findIndex(item => item.complaintCategoryId === category.complaintCategoryId);
    const next = [...this.categories];
    if (index < 0) next.push(category); else next[index] = category;
    this.categories = next.sort((first, second) => first.name.localeCompare(second.name));
  }
}
