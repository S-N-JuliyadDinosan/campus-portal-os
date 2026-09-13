import { Component } from '@angular/core';
import { ToastService } from '../../core/services/toast.service';
import { AppIconComponent } from './app-icon.component';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  imports: [AppIconComponent],
  template: `<div class="toast-host" aria-live="polite">@for(t of service.toasts();track t.id){<div class="toast" [class]="'toast '+t.tone"><app-icon [name]="t.tone==='success'?'check':'info'" [size]="18"/><div class="grow">{{t.message}}</div><button type="button" (click)="service.dismiss(t.id)" aria-label="Dismiss notification"><app-icon name="close" [size]="17"/></button></div>}</div>`
})
export class ToastHostComponent{constructor(public service:ToastService){}}
