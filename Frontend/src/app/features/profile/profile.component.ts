import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../core/services/student.service';
import { AuthService } from '../../core/services/auth.service';
import { StudentActivitySummary, StudentProfile } from '../../core/models/domain.models';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { LoadingComponent } from '../../shared/components/loading.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector:'app-profile', standalone:true,
  imports:[CommonModule,FormsModule,StatCardComponent,LoadingComponent],
  template:`
<div class="page">
  <div class="page-head"><div><h1>My profile</h1><p>Your verified university identity, contact details and account security.</p></div></div>
  @if(loading){<app-loading/>}
  @else if(error){<div class="alert error">{{error}}</div>}
  @else if(profile){
    <div class="grid grid-3">
      <div class="card">
        <div style="width:74px;height:74px;border-radius:22px;background:linear-gradient(135deg,#4f46e5,#7c3aed);color:white;display:grid;place-items:center;font-size:1.6rem;font-weight:800">{{profile.fullName[0]}}</div>
        <h2 style="margin-top:16px">{{profile.fullName}}</h2><div class="muted">{{profile.indexNumber}}</div><div class="divider"></div>
        <div class="stack small"><div><span class="muted">Faculty</span><br><strong>{{profile.facultyCode}} · {{profile.facultyName}}</strong></div><div><span class="muted">Email</span><br><strong>{{profile.email}}</strong></div><div><span class="muted">Account</span><br><span class="badge success">{{profile.isActive?'Active':'Inactive'}}</span></div></div>
      </div>
      <div class="card" style="grid-column:span 2">
        <h3 class="card-title">Contact details</h3><div class="card-sub">Index number, name, faculty and login email are controlled by university/admin records.</div>
        <form #f="ngForm" class="form-grid" style="margin-top:20px" (ngSubmit)="save()">
          <div class="field"><label for="profilePhone">Phone number</label><input class="input" id="profilePhone" name="phone" maxlength="30" [(ngModel)]="phone"></div>
          <div class="field full"><label for="profileAddress">Address</label><textarea class="textarea" id="profileAddress" name="address" maxlength="500" [(ngModel)]="address"></textarea></div>
          <div><button class="btn btn-primary" [disabled]="saving">{{saving?'Saving…':'Save changes'}}</button></div>
        </form>
      </div>
    </div>
    @if(activity){<div class="grid grid-3" style="margin-top:18px"><app-stat-card icon="⌂" [value]="activity.hostelApplications" label="Hostel applications"/><app-stat-card icon="⌘" [value]="activity.labBookings" label="Lab bookings"/><app-stat-card icon="◇" [value]="activity.eventRegistrations" label="Event registrations"/><app-stat-card icon="!" [value]="activity.complaints" label="Complaints"/><app-stat-card icon="▤" [value]="activity.certificateRequests" label="Certificates"/><app-stat-card icon="LKR" [value]="activity.outstandingFees" label="Outstanding fees"/></div>}
    <div class="card" style="margin-top:18px">
      <div class="row between mobile-stack"><div><h3 class="card-title">Account security</h3><div class="card-sub">Change your password. The backend revokes your current session after a successful change.</div></div><button class="btn btn-secondary" (click)="passwordOpen=!passwordOpen">{{passwordOpen?'Close':'Change password'}}</button></div>
      @if(passwordOpen){<form class="form-grid" #pf="ngForm" (ngSubmit)="changePassword()" style="margin-top:20px"><div class="field"><label for="currentPassword">Current password</label><input class="input" id="currentPassword" name="currentPassword" type="password" required [(ngModel)]="currentPassword"></div><div class="field"><label for="newPassword">New password</label><input class="input" id="newPassword" name="newPassword" type="password" required minlength="8" [(ngModel)]="newPassword"><span class="hint">Minimum 8 characters.</span></div><div class="field"><label for="confirmPassword">Confirm new password</label><input class="input" id="confirmPassword" name="confirmPassword" type="password" required [(ngModel)]="confirmPassword"><span class="field-error">@if(confirmPassword&&newPassword!==confirmPassword){Passwords do not match.}</span></div><div class="field" style="align-content:end"><button class="btn btn-primary" [disabled]="pf.invalid||changingPassword||newPassword!==confirmPassword">{{changingPassword?'Updating…':'Update password'}}</button></div></form>}
    </div>
  }
</div>`
})
export class ProfileComponent implements OnInit{
  loading=true;saving=false;error='';profile:StudentProfile|null=null;activity:StudentActivitySummary|null=null;phone='';address='';
  passwordOpen=false;currentPassword='';newPassword='';confirmPassword='';changingPassword=false;
  constructor(private students:StudentService,private auth:AuthService,private errors:ApiErrorService,private toast:ToastService){}
  ngOnInit(){
    this.students.me().subscribe({next:p=>{this.profile=p;this.phone=p.phoneNumber||'';this.address=p.address||'';this.loading=false},error:e=>{this.error=this.errors.message(e);this.loading=false}});
    this.students.activity().subscribe({next:a=>this.activity=a,error:()=>{}});
  }
  save(){this.saving=true;this.students.updateMe({phoneNumber:this.phone||null,address:this.address||null}).subscribe({next:p=>{this.profile=p;this.saving=false;this.toast.success('Profile updated.')},error:e=>{this.saving=false;this.toast.error(this.errors.message(e))}})}
  changePassword(){if(this.newPassword!==this.confirmPassword)return;this.changingPassword=true;this.auth.changePassword(this.currentPassword,this.newPassword).subscribe({next:()=>{this.changingPassword=false;this.toast.success('Password changed. Please sign in again.');this.auth.clear();setTimeout(()=>location.assign('/login'),600)},error:e=>{this.changingPassword=false;this.toast.error(this.errors.message(e))}})}
}
