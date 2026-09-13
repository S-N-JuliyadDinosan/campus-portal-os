import { Pipe, PipeTransform } from '@angular/core';
@Pipe({name:'statusLabel',standalone:true})
export class StatusLabelPipe implements PipeTransform{
  transform(value:string|number|null|undefined):string{
    if(value===null||value===undefined||value==='')return 'Unknown';
    return String(value).replace(/[_-]+/g,' ').replace(/([a-z])([A-Z])/g,'$1 $2').replace(/\b\w/g,m=>m.toUpperCase());
  }
}
