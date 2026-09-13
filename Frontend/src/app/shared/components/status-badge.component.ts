import { Component, Input } from '@angular/core';
import { StatusColorDirective } from '../directives/status-color.directive';
import { StatusLabelPipe } from '../pipes/status-label.pipe';
@Component({selector:'app-status-badge',standalone:true,imports:[StatusColorDirective,StatusLabelPipe],template:`<span [class]="'badge '+tone" [appStatusColor]="label">{{label|statusLabel}}</span>`})
export class StatusBadgeComponent{
 @Input() value:string|number|null|undefined='';
 get label(){const v=this.value;if(typeof v==='number')return String(v);return String(v??'Unknown').replace(/([a-z])([A-Z])/g,'$1 $2');}
 get tone(){const v=this.label.toLowerCase();if(['approved','resolved','paid','confirmed','active','ready','ready for collection','room assigned','collected'].some(x=>v.includes(x)))return'success';if(['rejected','cancelled','inactive','expired'].some(x=>v.includes(x)))return'danger';if(['pending','held','outstanding'].some(x=>v.includes(x)))return'warning';if(v.includes('in progress'))return'info';return'primary';}
}
