import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ComplaintService } from '../../core/services/portal-services';
import { Complaint, ComplaintCategory } from '../../core/models/domain.models';
import { StatusBadgeComponent } from '../../shared/components/status-badge.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ShortTextPipe } from '../../shared/pipes/short-text.pipe';
import { ApiErrorService } from '../../core/services/api-error.service';
import { ToastService } from '../../core/services/toast.service';

@Component({selector:'app-complaints',standalone:true,imports:[CommonModule,ReactiveFormsModule,StatusBadgeComponent,EmptyStateComponent,ShortTextPipe],template:`
<div class="page"><div class="page-head"><div><h1>Complaints & support</h1><p>Raise a service issue and follow it through Pending, In Progress and Resolved.</p></div><button class="btn btn-primary" (click)="showForm=!showForm">{{showForm?'Close':'Raise complaint'}}</button></div>
@if(showForm){<form class="card form-grid" [formGroup]="form" (ngSubmit)="submit()" style="margin-bottom:18px"><div class="field"><label for="complaintCategory">Category</label><select class="select" id="complaintCategory" formControlName="complaintCategoryId"><option [ngValue]="null">Choose a category</option>@for(c of categories;track c.complaintCategoryId){<option [ngValue]="c.complaintCategoryId">{{c.name}}</option>}</select></div><div class="field"><label>Privacy</label><label class="row"><input type="checkbox" formControlName="isAnonymous"> Submit anonymously where supported</label></div>
@if(categoryMode==='maintenance'){<div class="field"><label for="building">Building / location</label><input class="input" id="building" formControlName="building" placeholder="e.g. Engineering Block"></div><div class="field"><label for="room">Room / area</label><input class="input" id="room" formControlName="room" placeholder="e.g. Lab 03"></div>}
@if(categoryMode==='academic'){<div class="field"><label for="courseCode">Course code</label><input class="input" id="courseCode" formControlName="courseCode" placeholder="e.g. SE4012"></div>}
<div class="field full"><label for="complaintDescription">Description</label><textarea class="textarea" id="complaintDescription" formControlName="description" placeholder="Describe the issue clearly, including the impact and any relevant details."></textarea>@if(form.controls.description.touched&&form.controls.description.invalid){<span class="field-error">Please enter at least 10 characters.</span>}</div><div><button class="btn btn-primary" [disabled]="form.invalid||submitting">{{submitting?'Submitting…':'Submit complaint'}}</button></div></form>}
<div class="grid grid-3">@for(c of complaints;track c.complaintId){<article class="card"><div class="row between"><span class="badge info">{{c.categoryName}}</span><app-status-badge [value]="status(c.status)"/></div><p>{{c.description|shortText:180}}</p>@if(c.resolutionNote){<div class="alert success"><strong>Resolution note</strong><div class="small" style="margin-top:5px">{{c.resolutionNote}}</div></div>}<div class="small muted" style="margin-top:12px">Complaint #{{c.complaintId}}</div></article>}@empty{<div class="card" style="grid-column:1/-1"><app-empty-state icon="!" title="No complaints submitted" message="If you need help, raise a complaint and its progress will appear here."/></div>}</div></div>`})
export class ComplaintsComponent implements OnInit{
  private fb=inject(FormBuilder);
  categories:ComplaintCategory[]=[];complaints:Complaint[]=[];showForm=false;submitting=false;categoryMode:'maintenance'|'academic'|'general'='general';
  form=this.fb.group({complaintCategoryId:[null as number|null,Validators.required],isAnonymous:[false],description:['',[Validators.required,Validators.minLength(10)]],building:[''],room:[''],courseCode:['']});
  constructor(private service:ComplaintService,private errors:ApiErrorService,private toast:ToastService){
    this.form.controls.complaintCategoryId.valueChanges.subscribe(id=>this.configureDynamicFields(id));
  }
  ngOnInit(){this.service.categories().subscribe({next:x=>this.categories=x.filter(c=>c.isActive),error:e=>this.toast.error(this.errors.message(e))});this.load()}
  load(){this.service.mine().subscribe({next:x=>this.complaints=x,error:e=>this.toast.error(this.errors.message(e))})}
  status(n:number){return ({1:'Pending',2:'In Progress',3:'Resolved'} as Record<number,string>)[n]||String(n)}
  private configureDynamicFields(id:number|null){
    const name=(this.categories.find(c=>c.complaintCategoryId===id)?.name||'').toLowerCase();
    this.categoryMode=name.includes('maintenance')?'maintenance':name.includes('academic')?'academic':'general';
    const building=this.form.controls.building, room=this.form.controls.room, course=this.form.controls.courseCode;
    building.clearValidators();room.clearValidators();course.clearValidators();
    if(this.categoryMode==='maintenance'){building.setValidators([Validators.required,Validators.maxLength(120)]);room.setValidators([Validators.required,Validators.maxLength(120)])}
    if(this.categoryMode==='academic')course.setValidators([Validators.required,Validators.maxLength(80)]);
    building.updateValueAndValidity({emitEvent:false});room.updateValueAndValidity({emitEvent:false});course.updateValueAndValidity({emitEvent:false});
  }
  submit(){
    if(this.form.invalid)return;this.submitting=true;const v=this.form.getRawValue();let context='';
    if(this.categoryMode==='maintenance')context=`[Building: ${v.building} | Room/Area: ${v.room}]\n`;
    if(this.categoryMode==='academic')context=`[Course: ${v.courseCode}]\n`;
    this.service.create({complaintCategoryId:v.complaintCategoryId,description:context+(v.description||''),isAnonymous:!!v.isAnonymous}).subscribe({next:()=>{this.submitting=false;this.showForm=false;this.form.reset({complaintCategoryId:null,isAnonymous:false,description:'',building:'',room:'',courseCode:''});this.toast.success('Complaint submitted.');this.load()},error:e=>{this.submitting=false;this.toast.error(this.errors.message(e))}})
  }
}
