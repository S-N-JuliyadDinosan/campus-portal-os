import { Pipe, PipeTransform } from '@angular/core';
@Pipe({name:'timeRange',standalone:true})
export class TimeRangePipe implements PipeTransform{
  transform(start:string|null|undefined,end:string|null|undefined):string{
    const clean=(v:string|null|undefined)=>v?String(v).slice(0,5):'—';
    return `${clean(start)} – ${clean(end)}`;
  }
}
