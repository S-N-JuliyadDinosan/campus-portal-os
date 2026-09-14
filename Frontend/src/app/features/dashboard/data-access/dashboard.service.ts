import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { AdminDashboard, StudentDashboard } from '../models/dashboard.models';

const api=environment.apiUrl;

@Injectable({providedIn:'root'}) export class DashboardService{constructor(private h:HttpClient){} student(){return this.h.get<StudentDashboard>(`${api}/dashboard/student`)} admin(){return this.h.get<AdminDashboard>(`${api}/dashboard/admin`)}}
