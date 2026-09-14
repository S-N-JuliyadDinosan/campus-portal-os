import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { HostelService } from '../../core/services/portal-services';
import { Hostel, HostelApplication, Room } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-hostels-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, StatusBadgeComponent],
  template: `
    <div class="page">
      <div class="page-head">
        <div>
          <h1>Hostel administration</h1>
          <p>Review accommodation requests, manage hostel buildings and maintain room capacity.</p>
        </div>
      </div>

      <div class="tabs">
        <button class="tab" [class.active]="tab === 'applications'" (click)="tab = 'applications'; loadApplications()">Applications</button>
        <button class="tab" [class.active]="tab === 'hostels'" (click)="tab = 'hostels'; loadHostels()">Hostels & rooms</button>
      </div>

      @if (tab === 'applications') {
        <div class="card">
          <div class="toolbar">
            <div class="field">
              <label>Status</label>
              <select class="select" [(ngModel)]="status">
                <option value="">All statuses</option>
                <option>Pending</option>
                <option>Approved</option>
                <option>Rejected</option>
                <option>RoomAssigned</option>
                <option>Cancelled</option>
              </select>
            </div>
            <button class="btn btn-primary" (click)="loadApplications()">Apply filter</button>
          </div>

          <div class="table-wrap">
            <table class="table">
              <thead>
                <tr>
                  <th>Student</th><th>Hostel</th><th>Period</th><th>Status</th><th>Room</th><th>Requested</th><th></th>
                </tr>
              </thead>
              <tbody>
                @for (a of applications; track a.hostelApplicationId) {
                  <tr>
                    <td><strong>{{ a.studentName || 'Student' }}</strong><div class="small muted">{{ a.studentIndexNumber || a.studentId }}</div></td>
                    <td><strong>{{ a.preferredHostelName || ('Hostel #' + a.preferredHostelId) }}</strong></td>
                    <td>{{ a.academicYear }}<div class="small muted">{{ a.semester }}</div></td>
                    <td><app-status-badge [value]="a.status" /></td>
                    <td>{{ a.assignedRoomNumber || '—' }}</td>
                    <td>{{ a.requestedAt | date: 'mediumDate' }}</td>
                    <td class="right">
                      <div class="row wrap" style="justify-content:flex-end">
                        @if (a.status === 'Pending') {
                          <button class="btn btn-success btn-sm" (click)="setStatus(a, 'Approved')">Approve</button>
                          <button class="btn btn-danger btn-sm" (click)="setStatus(a, 'Rejected')">Reject</button>
                        }
                        @if (a.status === 'Approved') {
                          <button class="btn btn-primary btn-sm" (click)="openAssign(a)">Assign room</button>
                        }
                        @if (a.assignedRoomId) {
                          <button class="btn btn-secondary btn-sm" (click)="unassign(a)">Unassign</button>
                        }
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          @if (!applications.length) {
            <div class="empty">No hostel applications for this filter.</div>
          }
        </div>
      }

      @if (tab === 'hostels') {
        <div class="grid grid-3">
          <div class="card">
            <h3 class="card-title">{{ hostelEditId ? 'Edit' : 'Create' }} hostel</h3>
            <form class="stack" #hf="ngForm" (ngSubmit)="saveHostel(hf)" style="margin-top:16px">
              <div class="field">
                <label>Name</label>
                <input class="input" required name="name" [(ngModel)]="hostelForm.name">
              </div>
              <div class="field">
                <label>Location</label>
                <input class="input" name="location" [(ngModel)]="hostelForm.location">
              </div>
              @if (hostelEditId) {
                <label class="row"><input type="checkbox" name="active" [(ngModel)]="hostelForm.isActive"> Active</label>
              }
              <div class="row">
                <button class="btn btn-primary" [disabled]="hf.invalid || savingHostel">
                  {{ savingHostel ? 'Saving…' : 'Save' }}
                </button>
                @if (hostelEditId) {
                  <button type="button" class="btn btn-secondary" [disabled]="savingHostel" (click)="resetHostel(hf)">Cancel</button>
                }
              </div>
            </form>
          </div>

          <div class="card" style="grid-column:span 2">
            <div class="row between">
              <div>
                <h3 class="card-title">Hostels</h3>
                <div class="card-sub">Select a hostel to manage its rooms. Delete permanently removes the hostel and its related records.</div>
              </div>
            </div>
            <div class="divider"></div>
            <div class="stack">
              @for (h of hostels; track h.hostelId) {
                <div class="card flat" [style.background]="selectedHostel?.hostelId === h.hostelId ? '#eef2ff' : '#f8fafc'" (click)="selectHostel(h)" style="cursor:pointer">
                  <div class="row between mobile-stack">
                    <div>
                      <strong>{{ h.name }}</strong>
                      <div class="small muted">{{ h.location || 'No location' }}</div>
                    </div>
                    <div class="row wrap hostel-actions">
                      <app-status-badge [value]="h.isActive ? 'Active' : 'Inactive'" />
                      <button class="btn btn-secondary btn-sm" (click)="$event.stopPropagation(); editHostel(h)">Edit</button>
                      @if (h.isActive) {
                        <button class="btn btn-danger btn-sm" (click)="$event.stopPropagation(); deleteHostel(h)" [attr.aria-label]="'Delete ' + h.name">Delete</button>
                      }
                    </div>
                  </div>
                </div>
              }
              @if (!hostels.length) {
                <div class="empty">No hostels found.</div>
              }
            </div>
          </div>
        </div>

        @if (selectedHostel) {
          <div class="card" style="margin-top:18px">
            <div class="row between mobile-stack">
              <div>
                <h3 class="card-title">Rooms · {{ selectedHostel.name }}</h3>
                <div class="card-sub">Capacity is enforced when assigning accommodation.</div>
              </div>
              <button class="btn btn-primary" (click)="newRoom()">Add room</button>
            </div>
            <div class="divider"></div>
            <div class="table-wrap">
              <table class="table">
                <thead><tr><th>Room</th><th>Capacity</th><th>Status</th><th>Actions</th></tr></thead>
                <tbody>
                  @for (r of rooms; track r.roomId) {
                    <tr>
                      <td><strong>{{ r.roomNumber }}</strong></td>
                      <td>{{ r.capacity }}</td>
                      <td><app-status-badge [value]="r.isActive ? 'Active' : 'Inactive'" /></td>
                      <td class="right">
                        <div class="row wrap" style="justify-content:flex-end">
                          <button class="btn btn-secondary btn-sm" (click)="editRoom(r)">Edit</button>
                          @if (r.isActive) {
                            <button class="btn btn-danger btn-sm" (click)="deleteRoom(r)" [attr.aria-label]="'Delete room ' + r.roomNumber">Delete</button>
                          }
                        </div>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        }
      }

      @if (assignApp) {
        <div class="modal-backdrop" (click)="assignApp = null">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-head">
              <div>
                <h2>Assign room</h2>
                <div class="small muted">Application #{{ assignApp.hostelApplicationId }}</div>
              </div>
              <button class="icon-btn" (click)="assignApp = null">×</button>
            </div>
            <div class="field">
              <label>Available room</label>
              <select class="select" [(ngModel)]="assignRoomId">
                <option [ngValue]="null">Choose room</option>
                @for (r of assignRooms; track r.roomId) {
                  <option [ngValue]="r.roomId">{{ r.roomNumber }} · capacity {{ r.capacity }}</option>
                }
              </select>
            </div>
            <div class="row" style="margin-top:18px">
              <button class="btn btn-primary" [disabled]="!assignRoomId" (click)="assign()">Assign room</button>
              <button class="btn btn-secondary" (click)="assignApp = null">Cancel</button>
            </div>
          </div>
        </div>
      }

      @if (roomModal) {
        <div class="modal-backdrop" (click)="roomModal = false">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-head">
              <h2>{{ roomEditId ? 'Edit' : 'Add' }} room</h2>
              <button class="icon-btn" (click)="roomModal = false">×</button>
            </div>
            <form class="form-grid" #rf="ngForm" (ngSubmit)="saveRoom()">
              <div class="field">
                <label>Room number</label>
                <input class="input" required name="number" [(ngModel)]="roomForm.roomNumber">
              </div>
              <div class="field">
                <label>Capacity</label>
                <input class="input" type="number" min="1" required name="capacity" [(ngModel)]="roomForm.capacity">
              </div>
              @if (roomEditId) {
                <label class="row"><input type="checkbox" name="active" [(ngModel)]="roomForm.isActive"> Active</label>
              }
              <div class="row">
                <button class="btn btn-primary" [disabled]="rf.invalid">Save</button>
                <button type="button" class="btn btn-secondary" (click)="roomModal = false">Cancel</button>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  `
})
export class HostelsAdminComponent implements OnInit {
  tab: 'applications' | 'hostels' = 'applications';
  applications: HostelApplication[] = [];
  hostels: Hostel[] = [];
  rooms: Room[] = [];
  status = '';
  selectedHostel: Hostel | null = null;
  hostelEditId: number | null = null;
  savingHostel = false;
  hostelForm: { name: string; location: string; isActive: boolean } = {
    name: '',
    location: '',
    isActive: true
  };
  assignApp: HostelApplication | null = null;
  assignRooms: Room[] = [];
  assignRoomId: number | null = null;
  roomModal = false;
  roomEditId: number | null = null;
  roomForm: { roomNumber: string; capacity: number; isActive: boolean } = {
    roomNumber: '',
    capacity: 1,
    isActive: true
  };

  constructor(
    private service: HostelService,
    private errors: ApiErrorService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadApplications();
    this.loadHostels();
  }

  loadApplications() {
    this.service.applications(this.status || undefined).subscribe({
      next: applications => {
        this.applications = applications;
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  setStatus(application: HostelApplication, status: string) {
    if (!confirm(`${status} this application?`)) return;
    this.service.updateStatus(application.hostelApplicationId, status).subscribe({
      next: () => {
        this.toast.success(`Application ${status.toLowerCase()}.`);
        this.loadApplications();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  openAssign(application: HostelApplication) {
    this.assignApp = application;
    this.assignRoomId = null;
    this.service.rooms(application.preferredHostelId).subscribe({
      next: rooms => {
        this.assignRooms = rooms.filter(room => room.isActive);
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  assign() {
    if (!this.assignApp || !this.assignRoomId) return;
    this.service.assignRoom(this.assignApp.hostelApplicationId, this.assignRoomId).subscribe({
      next: () => {
        this.assignApp = null;
        this.toast.success('Room assigned.');
        this.loadApplications();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  unassign(application: HostelApplication) {
    if (!confirm('Unassign this room?')) return;
    this.service.unassignRoom(application.hostelApplicationId).subscribe({
      next: () => {
        this.toast.success('Room unassigned.');
        this.loadApplications();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  loadHostels() {
    this.service.hostels().subscribe({
      next: hostels => {
        this.hostels = this.sortedHostels(hostels);
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  editHostel(hostel: Hostel) {
    this.hostelEditId = hostel.hostelId;
    this.hostelForm = {
      name: hostel.name,
      location: hostel.location || '',
      isActive: hostel.isActive
    };
  }

  resetHostel(form?: NgForm) {
    this.hostelEditId = null;
    this.hostelForm = { name: '', location: '', isActive: true };
    form?.resetForm(this.hostelForm);
  }

  saveHostel(form: NgForm) {
    if (form.invalid || this.savingHostel) return;

    const name = this.hostelForm.name.trim();
    const location = this.hostelForm.location.trim() || null;
    const editId = this.hostelEditId;
    this.savingHostel = true;

    if (editId !== null) {
      const payload = { name, location, isActive: this.hostelForm.isActive };
      this.service.updateHostel(editId, payload).subscribe({
        next: () => {
          this.hostels = this.sortedHostels(
            this.hostels.map(hostel =>
              hostel.hostelId === editId ? { ...hostel, ...payload } : hostel
            )
          );
          if (this.selectedHostel?.hostelId === editId) {
            this.selectedHostel = { ...this.selectedHostel, ...payload };
          }
          this.finishHostelSave(form);
        },
        error: error => this.failHostelSave(error)
      });
      return;
    }

    this.service.createHostel({ name, location }).subscribe({
      next: created => {
        this.hostels = this.sortedHostels([
          ...this.hostels.filter(hostel => hostel.hostelId !== created.hostelId),
          created
        ]);
        this.finishHostelSave(form);
      },
      error: error => this.failHostelSave(error)
    });
  }

  deleteHostel(hostel: Hostel) {
    if (!confirm(`Permanently delete ${hostel.name} and its rooms and applications? This cannot be undone.`)) return;
    this.service.deleteHostel(hostel.hostelId).subscribe({
      next: () => {
        this.hostels = this.hostels.filter(item => item.hostelId !== hostel.hostelId);
        if (this.selectedHostel?.hostelId === hostel.hostelId) {
          this.selectedHostel = null;
          this.rooms = [];
        }
        this.toast.success('Hostel permanently deleted.');
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  selectHostel(hostel: Hostel) {
    this.selectedHostel = hostel;
    this.loadRooms();
  }

  loadRooms() {
    if (!this.selectedHostel) return;
    this.service.rooms(this.selectedHostel.hostelId).subscribe({
      next: rooms => {
        this.rooms = rooms.filter(room => room.isActive);
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  newRoom() {
    this.roomEditId = null;
    this.roomForm = { roomNumber: '', capacity: 1, isActive: true };
    this.roomModal = true;
  }

  editRoom(room: Room) {
    this.roomEditId = room.roomId;
    this.roomForm = {
      roomNumber: room.roomNumber,
      capacity: room.capacity,
      isActive: room.isActive
    };
    this.roomModal = true;
  }

  saveRoom() {
    if (!this.selectedHostel) return;
    const request = this.roomEditId
      ? this.service.updateRoom(this.roomEditId, this.roomForm)
      : this.service.createRoom(this.selectedHostel.hostelId, {
          roomNumber: this.roomForm.roomNumber,
          capacity: this.roomForm.capacity
        });
    request.subscribe({
      next: () => {
        this.roomModal = false;
        this.toast.success('Room saved.');
        this.loadRooms();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  deleteRoom(room: Room) {
    if (!confirm(`Delete room ${room.roomNumber} from active rooms?`)) return;
    this.service.deleteRoom(room.roomId).subscribe({
      next: () => {
        this.rooms = this.rooms.filter(item => item.roomId !== room.roomId);
        this.toast.success('Room deleted.');
        this.cdr.markForCheck();
      },
      error: error => this.toast.error(this.errors.message(error))
    });
  }

  private sortedHostels(hostels: Hostel[]): Hostel[] {
    return hostels
      .sort((left, right) => left.name.localeCompare(right.name));
  }

  private finishHostelSave(form: NgForm) {
    this.savingHostel = false;
    this.resetHostel(form);
    this.toast.success('Hostel saved.');
    this.cdr.markForCheck();
  }

  private failHostelSave(error: unknown) {
    this.savingHostel = false;
    this.toast.error(this.errors.message(error));
    this.cdr.markForCheck();
  }
}
