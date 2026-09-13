import { Directive, ElementRef, HostListener, Renderer2 } from '@angular/core';

@Directive({selector:'[appHighlight]',standalone:true})
export class HighlightDirective{
  constructor(private el:ElementRef<HTMLElement>,private renderer:Renderer2){}
  @HostListener('mouseenter') enter(){
    this.renderer.setStyle(this.el.nativeElement,'transform','translateY(-2px)');
    this.renderer.setStyle(this.el.nativeElement,'box-shadow','0 12px 28px rgba(16,24,40,.08)');
    this.renderer.setStyle(this.el.nativeElement,'border-color','#d4dcf1');
  }
  @HostListener('mouseleave') leave(){
    this.renderer.removeStyle(this.el.nativeElement,'transform');
    this.renderer.removeStyle(this.el.nativeElement,'box-shadow');
    this.renderer.removeStyle(this.el.nativeElement,'border-color');
  }
}
