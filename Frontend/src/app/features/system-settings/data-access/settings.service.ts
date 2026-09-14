import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { SystemSetting } from '../models/system-setting.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class SettingsService{constructor(private h:HttpClient){} all(){return this.h.get<SystemSetting[]>(`${api}/admin/system-settings`)} update(key:string,settingValue:string){return this.h.put<SystemSetting>(`${api}/admin/system-settings/${encodeURIComponent(key)}`,{settingValue})} holdMinutes(){return this.h.get<{minutes:number}>(`${api}/admin/system-settings/reservation-hold-minutes`)} updateHold(minutes:number){return this.h.put<{minutes:number}>(`${api}/admin/system-settings/reservation-hold-minutes`,{minutes})}}
