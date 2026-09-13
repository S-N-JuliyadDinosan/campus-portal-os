import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { studentGuard } from './core/guards/student.guard';

export const routes: Routes = [
  { path:'login', loadComponent:()=>import('./features/auth/login.component').then(m=>m.LoginComponent) },
  { path:'register', loadComponent:()=>import('./features/auth/register.component').then(m=>m.RegisterComponent) },
  { path:'forgot-password', loadComponent:()=>import('./features/auth/forgot-password.component').then(m=>m.ForgotPasswordComponent) },
  { path:'reset-password', loadComponent:()=>import('./features/auth/reset-password.component').then(m=>m.ResetPasswordComponent) },
  {
    path:'', canActivate:[authGuard], loadComponent:()=>import('./layout/app-shell.component').then(m=>m.AppShellComponent), children:[
      { path:'', pathMatch:'full', redirectTo:'dashboard' },
      { path:'dashboard', loadComponent:()=>import('./features/dashboard/dashboard.component').then(m=>m.DashboardComponent) },
      { path:'profile', canActivate:[studentGuard], loadComponent:()=>import('./features/profile/profile.component').then(m=>m.ProfileComponent) },
      { path:'hostels', canActivate:[studentGuard], loadComponent:()=>import('./features/hostels/hostels.component').then(m=>m.HostelsComponent) },
      { path:'labs', canActivate:[studentGuard], loadComponent:()=>import('./features/labs/labs.component').then(m=>m.LabsComponent) },
      { path:'events', canActivate:[studentGuard], loadComponent:()=>import('./features/events/events.component').then(m=>m.EventsComponent) },
      { path:'complaints', canActivate:[studentGuard], loadComponent:()=>import('./features/complaints/complaints.component').then(m=>m.ComplaintsComponent) },
      { path:'certificates', canActivate:[studentGuard], loadComponent:()=>import('./features/certificates/certificates.component').then(m=>m.CertificatesComponent) },
      { path:'fees', canActivate:[studentGuard], loadComponent:()=>import('./features/fees/fees.component').then(m=>m.FeesComponent) },
      { path:'fees/:id/receipt', canActivate:[studentGuard], loadComponent:()=>import('./features/fees/receipt.component').then(m=>m.ReceiptComponent) },
      { path:'notifications', canActivate:[studentGuard], loadComponent:()=>import('./features/notifications/notifications.component').then(m=>m.NotificationsComponent) },
      { path:'admin/students', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/students-admin.component').then(m=>m.StudentsAdminComponent) },
      { path:'admin/hostels', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/hostels-admin.component').then(m=>m.HostelsAdminComponent) },
      { path:'admin/labs', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/labs-admin.component').then(m=>m.LabsAdminComponent) },
      { path:'admin/events', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/events-admin.component').then(m=>m.EventsAdminComponent) },
      { path:'admin/complaints', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/complaints-admin.component').then(m=>m.ComplaintsAdminComponent) },
      { path:'admin/certificates', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/certificates-admin.component').then(m=>m.CertificatesAdminComponent) },
      { path:'admin/fees', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/fees-admin.component').then(m=>m.FeesAdminComponent) },
      { path:'admin/settings', canActivate:[adminGuard], loadComponent:()=>import('./features/admin/settings-admin.component').then(m=>m.SettingsAdminComponent) }
    ]
  },
  { path:'**', loadComponent:()=>import('./features/auth/not-found.component').then(m=>m.NotFoundComponent) }
];
