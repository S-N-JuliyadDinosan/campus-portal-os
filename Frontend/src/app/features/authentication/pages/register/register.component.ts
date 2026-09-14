import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { StudentService } from '../../../students/data-access/student.service';
import { ApiErrorService } from '../../../../core/http/api-error.service';
import { ToastService } from '../../../../core/services/toast.service';
import { StudentMasterCheck } from '../../../students/models/student.models';
import { authStyles } from '../../styles/auth-layout.styles';

const indexNumberValidator:ValidatorFn=(control:AbstractControl):ValidationErrors|null=>{
  const value=String(control.value??'').trim();
  if(!value)return null;
  return value.length>=3 && !/\s/.test(value)?null:{indexNumber:true};
};
const passwordMatchValidator:ValidatorFn=(control:AbstractControl):ValidationErrors|null=>{
  const p=control.get('password')?.value;
  const c=control.get('confirmPassword')?.value;
  return p&&c&&p!==c?{passwordMismatch:true}:null;
};

@Component({
  selector:'app-register',standalone:true,imports:[CommonModule,ReactiveFormsModule,RouterLink],
  template:`
<div class="auth-page">
  <section class="auth-art"><div class="auth-brand"><div class="auth-logo">C</div><div><strong>Campus Services</strong><span>Student onboarding</span></div></div><div class="auth-copy"><span style="color:#c4b5fd;font-size:.78rem;font-weight:800;letter-spacing:.14em;text-transform:uppercase">Get started</span><h1>Your campus account starts with your index number.</h1><p>The portal checks your university student-master record first, then creates a secure student account that connects to every service.</p></div><div class="auth-features"><span class="auth-chip">Verified identity</span><span class="auth-chip">Reactive validation</span><span class="auth-chip">Secure account</span></div></section>
  <section class="auth-main">
    <form class="auth-card" [formGroup]="form" (ngSubmit)="submit()"><h2>Create student account</h2><p>Use the details registered with your university.</p>
      @if(error){<div class="alert error" style="margin-bottom:16px">{{error}}</div>}
      <div class="stack">
        <div class="field"><label for="indexNumber">Index number</label><div class="index-check"><input class="input" id="indexNumber" formControlName="indexNumber" maxlength="50" placeholder="e.g. SE/2026/001"><button type="button" class="btn btn-secondary" [disabled]="form.controls.indexNumber.invalid||checking" (click)="checkIndex()">{{checking?'Checking…':'Check'}}</button></div>@if(form.controls.indexNumber.touched&&form.controls.indexNumber.hasError('indexNumber')){<span class="field-error">Enter a valid index number without spaces.</span>}@if(master){<div class="master-box">@if(master.exists&&master.isActive&&!master.alreadyRegistered){✓ Active student record found. You can continue registration.}@else if(master.alreadyRegistered){This index number already has an account.}@else if(master.exists&&!master.isActive){This student record is inactive.}@else{No master record was found. Enter your full name and backend Faculty ID below.}</div>}</div>
        <div class="field"><label for="registerEmail">Email</label><input class="input" id="registerEmail" type="email" formControlName="email" placeholder="Official email">@if(form.controls.email.touched&&form.controls.email.invalid){<span class="field-error">Enter a valid email address.</span>}</div>
        @if(master&&!master.exists){<div class="form-grid"><div class="field"><label for="fullName">Full name</label><input class="input" id="fullName" formControlName="fullName"></div><div class="field"><label for="facultyId">Faculty ID</label><input class="input" id="facultyId" type="number" min="1" formControlName="facultyId"><span class="hint">The backend requires a valid active Faculty ID when the index number is not preloaded in the student master list.</span></div></div>}
        <div class="form-grid"><div class="field"><label for="phone">Phone number</label><input class="input" id="phone" formControlName="phoneNumber" maxlength="30" placeholder="Optional"></div><div class="field"><label for="password">Password</label><input class="input" id="password" type="password" formControlName="password" placeholder="Minimum 8 characters">@if(form.controls.password.touched&&form.controls.password.invalid){<span class="field-error">Password must contain at least 8 characters.</span>}</div><div class="field"><label for="confirmPassword">Confirm password</label><input class="input" id="confirmPassword" type="password" formControlName="confirmPassword">@if(form.hasError('passwordMismatch')&&form.controls.confirmPassword.touched){<span class="field-error">Passwords do not match.</span>}</div><div class="field full"><label for="address">Address</label><textarea class="textarea" id="address" formControlName="address" maxlength="500" placeholder="Optional"></textarea></div></div>
        <button class="btn btn-primary" [disabled]="form.invalid||loading||!master||master.alreadyRegistered||(master.exists&&!master.isActive)">{{loading?'Creating account…':'Create account'}}</button>
      </div><div class="auth-footer">Already registered? <a class="auth-link" routerLink="/login">Sign in</a></div>
    </form>
  </section>
</div>`,styles:[authStyles]
})
export class RegisterComponent{
  private fb=inject(FormBuilder);
  master:StudentMasterCheck|null=null;checking=false;loading=false;error='';
  form=this.fb.group({
    indexNumber:['',[Validators.required,Validators.maxLength(50),indexNumberValidator]],
    email:['',[Validators.required,Validators.email,Validators.maxLength(256)]],
    fullName:[''],facultyId:[null as number|null],phoneNumber:['',[Validators.maxLength(30)]],address:['',[Validators.maxLength(500)]],
    password:['',[Validators.required,Validators.minLength(8)]],confirmPassword:['',[Validators.required]]
  },{validators:passwordMatchValidator});
  constructor(private students:StudentService,private errors:ApiErrorService,private toast:ToastService,private router:Router,private cdr:ChangeDetectorRef){
    this.form.controls.indexNumber.valueChanges.subscribe(()=>{this.master=null;this.syncFallbackValidators(false)});
  }
  private syncFallbackValidators(required:boolean){
    const name=this.form.controls.fullName, faculty=this.form.controls.facultyId;
    name.setValidators(required?[Validators.required,Validators.maxLength(200)]:[]);
    faculty.setValidators(required?[Validators.required,Validators.min(1)]:[]);
    name.updateValueAndValidity({emitEvent:false});faculty.updateValueAndValidity({emitEvent:false});
  }
  checkIndex(){
    if(this.form.controls.indexNumber.invalid)return;
    this.checking=true;this.error='';
    this.cdr.detectChanges();
    this.students.checkMaster(this.form.controls.indexNumber.value!.trim()).subscribe({
      next:r=>{
        this.master=r;
        this.syncFallbackValidators(!r.exists);
        this.checking=false;
        this.cdr.markForCheck();
      },
      error:e=>{
        this.checking=false;
        this.error=this.errors.message(e);
        this.cdr.markForCheck();
      }
    });
  }
  submit(){
    if(this.form.invalid||!this.master)return;
    this.loading=true;this.error='';const v=this.form.getRawValue();
    const body={indexNumber:v.indexNumber!.trim(),email:v.email!.trim(),password:v.password!,phoneNumber:v.phoneNumber?.trim()||null,address:v.address?.trim()||null,fullName:!this.master.exists?v.fullName?.trim()||null:null,facultyId:!this.master.exists?v.facultyId:null};
    this.students.register(body).subscribe({next:r=>{this.loading=false;if(r.success){this.toast.success('Account created. You can sign in now.');void this.router.navigate(['/login']);}else this.error=r.message;this.cdr.markForCheck()},error:e=>{this.loading=false;this.error=this.errors.message(e,'Registration failed.');this.cdr.markForCheck()}});
  }
}
