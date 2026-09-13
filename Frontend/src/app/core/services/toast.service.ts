import { Injectable, signal } from '@angular/core';
export type ToastTone = 'success' | 'error' | 'info' | 'warning';
export interface Toast { id:number; message:string; tone:ToastTone; }
@Injectable({providedIn:'root'})
export class ToastService {
  private nextId = 1;
  readonly toasts = signal<Toast[]>([]);
  show(message:string, tone:ToastTone='info', ms=3500){
    const toast={id:this.nextId++,message,tone};
    this.toasts.update(v=>[...v,toast]);
    window.setTimeout(()=>this.dismiss(toast.id),ms);
  }
  success(m:string){this.show(m,'success');}
  error(m:string){this.show(m,'error',5000);}
  warning(m:string){this.show(m,'warning',4500);}
  dismiss(id:number){this.toasts.update(v=>v.filter(t=>t.id!==id));}
}
