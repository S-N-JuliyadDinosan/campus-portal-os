import { Directive, ElementRef, Input, OnChanges, Renderer2 } from '@angular/core';

@Directive({selector:'[appStatusColor]',standalone:true})
export class StatusColorDirective implements OnChanges{
  @Input() appStatusColor:string|number|null|undefined='';
  constructor(private el:ElementRef<HTMLElement>,private renderer:Renderer2){}
  ngOnChanges(){
    const v=String(this.appStatusColor??'').replace(/([a-z])([A-Z])/g,'$1 $2').toLowerCase();
    const color=v.includes('approved')||v.includes('resolved')||v.includes('paid')||v.includes('confirmed')||v.includes('active')||v.includes('ready')||v.includes('assigned')||v.includes('collected')?'#087a55':v.includes('rejected')||v.includes('cancelled')||v.includes('inactive')||v.includes('expired')?'#c43232':v.includes('pending')||v.includes('held')||v.includes('outstanding')?'#b85c00':v.includes('progress')?'#1769aa':'#2855d9';
    this.renderer.setStyle(this.el.nativeElement,'--status-accent',color);
  }
}
