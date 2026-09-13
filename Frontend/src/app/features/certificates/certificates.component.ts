import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormArray, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CertificateService } from '../../core/services/portal-services';
import { CertificateRequest, CertificateType } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

type CertificateRowForm=FormGroup<{certificateTypeId:FormControl<number|null>;reason:FormControl<string>}>;

const noDuplicateCertificateTypes:ValidatorFn=(control:AbstractControl):ValidationErrors|null=>{
  const rows=(control as FormArray<CertificateRowForm>).controls;
  const ids=rows.map(r=>r.controls.certificateTypeId.value).filter((id):id is number=>id!==null);
  return new Set(ids).size===ids.length?null:{duplicateType:true};
};

@Component({selector:'app-certificates',standalone:true,imports:[CommonModule,ReactiveFormsModule,StatusBadgeComponent,EmptyStateComponent],template:`
<div class="page"><div class="page-head"><div><h1>Certificate requests</h1><p>Request one or several official university documents and track approval or collection readiness.</p></div><button class="btn btn-primary" (click)="toggleForm()">{{showForm?'Close':'New request'}}</button></div>
@if(showForm){<form class="card" [formGroup]="form" (ngSubmit)="submit()" style="margin-bottom:18px"><div class="row between mobile-stack"><div><h3 class="card-title">Documents to request</h3><div class="card-sub">Each row is submitted to the backend as an individual certificate request.</div></div><button type="button" class="btn btn-secondary btn-sm" (click)="addRow()">Add document</button></div><div class="divider"></div><div formArrayName="documents" class="stack">@for(row of documents.controls;track $index;let i=$index){<div class="card flat" [formGroupName]="i" style="background:#f8fafc"><div class="form-grid"><div class="field"><label [for]="'certType'+i">Certificate type</label><select class="select" [id]="'certType'+i" formControlName="certificateTypeId"><option [ngValue]="null">Choose document</option>@for(t of types;track t.certificateTypeId){<option [ngValue]="t.certificateTypeId">{{t.name}}</option>}</select></div><div class="field full"><label [for]="'certReason'+i">Reason</label><textarea class="textarea" [id]="'certReason'+i" maxlength="500" formControlName="reason" placeholder="Optional reason or intended use"></textarea></div></div>@if(documents.length>1){<div class="right" style="margin-top:10px"><button type="button" class="btn btn-danger btn-sm" (click)="removeRow(i)">Remove</button></div>}</div>}</div>@if(documents.hasError('duplicateType')){<div class="alert error" style="margin-top:14px">The same certificate type cannot be added twice in one submission.</div>}<div class="row" style="margin-top:18px"><button class="btn btn-primary" [disabled]="form.invalid||submitting">{{submitting?'Submitting…':'Submit request'+(documents.length>1?'s':'')}}</button></div></form>}
<div class="grid grid-3">@for(r of requests;track r.certificateRequestId){<article class="card"><div class="row between"><span class="badge primary">#{{r.certificateRequestId}}</span><app-status-badge [value]="r.status"/></div><h3 style="margin-top:14px">{{r.certificateTypeName}}</h3><div class="small muted">Requested {{r.requestedAt|date:'mediumDate'}}</div>@if(r.reason){<p class="small">{{r.reason}}</p>}@if(r.reviewNote){<div class="alert info"><strong>Review note</strong><div class="small" style="margin-top:5px">{{r.reviewNote}}</div></div>}</article>}@empty{<div class="card" style="grid-column:1/-1"><app-empty-state icon="▤" title="No certificate requests" message="Request a configured official document to see its status here."/></div>}</div></div>`})
export class CertificatesComponent implements OnInit{
  types:CertificateType[]=[];requests:CertificateRequest[]=[];showForm=false;submitting=false;
  documents=new FormArray<CertificateRowForm>([],noDuplicateCertificateTypes);
  form=new FormGroup({documents:this.documents});
  constructor(private service:CertificateService,private errors:ApiErrorService,private toast:ToastService){}
  ngOnInit(){this.service.types().subscribe({next:x=>this.types=x.filter(t=>t.isActive),error:e=>this.toast.error(this.errors.message(e))});this.load();this.addRow()}
  toggleForm(){this.showForm=!this.showForm;if(this.showForm&&this.documents.length===0)this.addRow()}
  addRow(){this.documents.push(new FormGroup({certificateTypeId:new FormControl<number|null>(null,{validators:[Validators.required]}),reason:new FormControl('',{nonNullable:true,validators:[Validators.maxLength(500)]})}))}
  removeRow(i:number){this.documents.removeAt(i)}
  load(){this.service.mine().subscribe({next:x=>this.requests=x,error:e=>this.toast.error(this.errors.message(e))})}
  submit(){if(this.form.invalid)return;this.submitting=true;const calls=this.documents.getRawValue().map(x=>this.service.create(x.certificateTypeId!,x.reason?.trim()||undefined));forkJoin(calls).subscribe({next:()=>{this.submitting=false;this.showForm=false;this.documents.clear();this.addRow();this.toast.success('Certificate request(s) submitted.');this.load()},error:e=>{this.submitting=false;this.toast.error(this.errors.message(e))}})}
}
