import { Pipe, PipeTransform } from '@angular/core';
@Pipe({name:'shortText',standalone:true}) export class ShortTextPipe implements PipeTransform{transform(v:string|null|undefined,n=80){if(!v)return'';return v.length>n?v.slice(0,n).trimEnd()+'…':v;}}
