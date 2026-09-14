import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { adminGuard } from './core/auth/guards/admin.guard';
import { studentGuard } from './core/auth/guards/student.guard';

export const routes: Routes = [
  { path:'login', loadComponent:()=>import('./features/authentication/pages/login/login.component').then(m=>m.LoginComponent) },
  { path:'register', loadComponent:()=>import('./features/authentication/pages/register/register.component').then(m=>m.RegisterComponent) },
  { path:'forgot-password', loadComponent:()=>import('./features/authentication/pages/forgot-password/forgot-password.component').then(m=>m.ForgotPasswordComponent) },
  { path:'reset-password', loadComponent:()=>import('./features/authentication/pages/reset-password/reset-password.component').then(m=>m.ResetPasswordComponent) },
  {
    path:'', canActivate:[authGuard], loadComponent:()=>import('./layout/app-shell/app-shell.component').then(m=>m.AppShellComponent), children:[
      { path:'', pathMatch:'full', redirectTo:'dashboard' },
      { path:'dashboard', loadComponent:()=>import('./features/dashboard/pages/dashboard/dashboard.component').then(m=>m.DashboardComponent) },
      { path:'profile', canActivate:[studentGuard], loadComponent:()=>import('./features/students/pages/student-profile/profile.component').then(m=>m.ProfileComponent) },
      { path:'hostels', canActivate:[studentGuard], loadComponent:()=>import('./features/hostels/pages/student-hostels/hostels.component').then(m=>m.HostelsComponent) },
      { path:'labs', canActivate:[studentGuard], loadComponent:()=>import('./features/labs/pages/student-labs/labs.component').then(m=>m.LabsComponent) },
      { path:'events', canActivate:[studentGuard], loadComponent:()=>import('./features/events/pages/student-events/events.component').then(m=>m.EventsComponent) },
      { path:'complaints', canActivate:[studentGuard], loadComponent:()=>import('./features/complaints/pages/student-complaints/complaints.component').then(m=>m.ComplaintsComponent) },
      { path:'certificates', canActivate:[studentGuard], loadComponent:()=>import('./features/certificates/pages/student-certificates/certificates.component').then(m=>m.CertificatesComponent) },
      { path:'fees', canActivate:[studentGuard], loadComponent:()=>import('./features/fees/pages/student-fees/fees.component').then(m=>m.FeesComponent) },
      { path:'fees/:id/receipt', canActivate:[studentGuard], loadComponent:()=>import('./features/fees/pages/payment-receipt/receipt.component').then(m=>m.ReceiptComponent) },
      { path:'notifications', canActivate:[studentGuard], loadComponent:()=>import('./features/notifications/pages/notifications/notifications.component').then(m=>m.NotificationsComponent) },
      { path:'admin/students', canActivate:[adminGuard], loadComponent:()=>import('./features/students/pages/admin-students/students-admin.component').then(m=>m.StudentsAdminComponent) },
      { path:'admin/hostels', canActivate:[adminGuard], loadComponent:()=>import('./features/hostels/pages/admin-hostels/hostels-admin.component').then(m=>m.HostelsAdminComponent) },
      { path:'admin/labs', canActivate:[adminGuard], loadComponent:()=>import('./features/labs/pages/admin-labs/labs-admin.component').then(m=>m.LabsAdminComponent) },
      { path:'admin/events', canActivate:[adminGuard], loadComponent:()=>import('./features/events/pages/admin-events/events-admin.component').then(m=>m.EventsAdminComponent) },
      { path:'admin/complaints', canActivate:[adminGuard], loadComponent:()=>import('./features/complaints/pages/admin-complaints/complaints-admin.component').then(m=>m.ComplaintsAdminComponent) },
      { path:'admin/certificates', canActivate:[adminGuard], loadComponent:()=>import('./features/certificates/pages/admin-certificates/certificates-admin.component').then(m=>m.CertificatesAdminComponent) },
      { path:'admin/fees', canActivate:[adminGuard], loadComponent:()=>import('./features/fees/pages/admin-fees/fees-admin.component').then(m=>m.FeesAdminComponent) },
      { path:'admin/settings', canActivate:[adminGuard], loadComponent:()=>import('./features/system-settings/pages/admin-settings/settings-admin.component').then(m=>m.SettingsAdminComponent) }
    ]
  },
  { path:'**', loadComponent:()=>import('./features/authentication/pages/not-found/not-found.component').then(m=>m.NotFoundComponent) }
];
