import { Component, HostListener, OnInit } from '@angular/core';
import {
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../core/services/auth.service';
import { NotificationService } from '../core/services/portal-services';
import { AppIconComponent } from '../shared/components/app-icon.component';

interface NavigationItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AppIconComponent],
  template: `
    <div class="shell">
      <aside class="app-sidebar" [class.open]="menuOpen" aria-label="Primary navigation">
        <div class="brand">
          <div class="brand-mark"><app-icon name="campus" [size]="24" [strokeWidth]="1.9"/></div>
          <div class="brand-copy">
            <strong>Campus Services</strong>
            <span>Services Portal</span>
          </div>
          <button class="sidebar-close" type="button" (click)="menuOpen=false" aria-label="Close navigation">
            <app-icon name="close" [size]="19"/>
          </button>
        </div>

        <div class="workspace-label">{{auth.role()==='Admin' ? 'Administration' : 'Student workspace'}}</div>
        <nav>
          @for (item of menu; track item.path) {
            <a
              [routerLink]="item.path"
              routerLinkActive="active"
              [routerLinkActiveOptions]="{exact: item.path === '/dashboard'}"
              (click)="menuOpen=false"
            >
              <span class="nav-icon"><app-icon [name]="item.icon" [size]="19"/></span>
              <span>{{item.label}}</span>
              @if (item.label==='Notifications' && notifications.unread()>0) {
                <b class="nav-count">{{notifications.unread() > 99 ? '99+' : notifications.unread()}}</b>
              }
            </a>
          }
        </nav>

        <div class="sidebar-support">
          <div class="support-icon"><app-icon name="shield" [size]="18"/></div>
          <div><strong>Secure portal</strong><span>Your session is protected</span></div>
        </div>

        <div class="side-footer">
          <div class="avatar">{{initial}}</div>
          <div class="user-copy grow">
            <strong>{{displayName}}</strong>
            <span>{{auth.user().email}}</span>
          </div>
          <button class="logout" type="button" (click)="auth.logout()" title="Sign out" aria-label="Sign out">
            <app-icon name="logout" [size]="18"/>
          </button>
        </div>
      </aside>

      <div class="app-content">
        @if (navigating) {
          <div class="nav-progress" role="progressbar" aria-label="Loading page"><span></span></div>
        }
        <header class="app-topbar">
          <div class="topbar-left">
            <button class="menu-btn" type="button" (click)="menuOpen=!menuOpen" aria-label="Open navigation">
              <app-icon name="menu" [size]="21"/>
            </button>
            <div class="page-context">
              <span>Campus portal</span>
              <b>{{pageTitle}}</b>
            </div>
          </div>
          <div class="top-actions">
            <div class="date-chip">
              <app-icon name="clock" [size]="16"/>
              <span>{{today}}</span>
            </div>
            @if (auth.role()==='Student') {
              <a routerLink="/notifications" class="top-icon-button" aria-label="Notifications">
                <app-icon name="bell" [size]="19"/>
                @if (notifications.unread()>0) { <i>{{notifications.unread() > 99 ? '99+' : notifications.unread()}}</i> }
              </a>
            }
            <div class="top-user" title="{{auth.user().email}}">
              <div class="mini-avatar">{{initial}}</div>
              <div><strong>{{displayName}}</strong><span>{{auth.role()}}</span></div>
            </div>
          </div>
        </header>
        <main class="app-main"><router-outlet/></main>
      </div>

      @if (menuOpen) { <button class="mobile-overlay" type="button" (click)="menuOpen=false" aria-label="Close navigation"></button> }
    </div>
  `,
  styles: [`
    .shell { min-height: 100vh; }
    .app-sidebar {
      position: fixed;
      z-index: 1000;
      inset: 0 auto 0 0;
      display: flex;
      flex-direction: column;
      width: 258px;
      padding: 18px 13px 14px;
      overflow: hidden;
      background:
        radial-gradient(circle at 15% 0, rgba(76, 114, 211, .14), transparent 19rem),
        #111a2c;
      color: #c4ccda;
      border-right: 1px solid rgba(255,255,255,.055);
    }
    .brand { display: flex; align-items: center; gap: 11px; padding: 2px 7px 25px; }
    .brand-mark {
      display: grid;
      place-items: center;
      flex: 0 0 auto;
      width: 42px;
      height: 42px;
      background: #315fdc;
      color: #fff;
      border: 1px solid rgba(255,255,255,.15);
      border-radius: 12px;
      box-shadow: 0 8px 20px rgba(25,65,174,.28);
    }
    .brand-copy strong { display: block; color: #fff; font-family: Manrope, sans-serif; font-size: 1.02rem; letter-spacing: -.02em; }
    .brand-copy span { display: block; margin-top: 1px; color: #8390a7; font-size: .68rem; letter-spacing: .025em; }
    .sidebar-close { display: none; margin-left: auto; padding: 5px; background: transparent; color: #aab4c5; border: 0; }
    .workspace-label { padding: 0 11px 9px; color: #697891; font-size: .62rem; font-weight: 700; letter-spacing: .11em; text-transform: uppercase; }
    nav { display: grid; gap: 3px; padding: 0 1px 10px; overflow: auto; scrollbar-width: thin; scrollbar-color: #29364d transparent; }
    nav a {
      position: relative;
      display: flex;
      align-items: center;
      gap: 11px;
      min-height: 43px;
      padding: 9px 10px;
      color: #aab5c7;
      border: 1px solid transparent;
      border-radius: 9px;
      font-size: .82rem;
      font-weight: 600;
      transition: background .15s ease, color .15s ease, border-color .15s ease;
    }
    nav a:hover { background: rgba(255,255,255,.045); color: #f4f7fb; }
    nav a.active { background: rgba(55, 96, 211, .18); color: #fff; border-color: rgba(108, 143, 239, .16); }
    nav a.active::before { position: absolute; inset: 9px auto 9px -2px; width: 3px; background: #78a0ff; border-radius: 0 4px 4px 0; content: ''; }
    .nav-icon { display: grid; place-items: center; flex: 0 0 auto; width: 25px; color: #8392aa; }
    nav a.active .nav-icon { color: #8fb0ff; }
    .nav-count { display: grid; place-items: center; min-width: 20px; height: 20px; margin-left: auto; padding: 0 5px; background: #e5484d; color: #fff; border-radius: 10px; font-size: .61rem; }
    .sidebar-support { display: flex; align-items: center; gap: 10px; margin: auto 2px 13px; padding: 12px; background: rgba(255,255,255,.035); border: 1px solid rgba(255,255,255,.06); border-radius: 11px; }
    .support-icon { display: grid; place-items: center; width: 32px; height: 32px; background: rgba(62,105,220,.17); color: #8eaeff; border-radius: 8px; }
    .sidebar-support strong { display: block; color: #dce3ee; font-size: .72rem; }
    .sidebar-support span { display: block; margin-top: 2px; color: #77869d; font-size: .62rem; }
    .side-footer { display: flex; align-items: center; gap: 9px; padding: 14px 5px 1px; border-top: 1px solid #263147; }
    .avatar, .mini-avatar { display: grid; place-items: center; flex: 0 0 auto; background: #dfe8ff; color: #254aae; border-radius: 50%; font-weight: 800; }
    .avatar { width: 36px; height: 36px; font-size: .78rem; }
    .mini-avatar { width: 34px; height: 34px; font-size: .72rem; }
    .user-copy strong { display: block; overflow: hidden; color: #f3f5f8; font-size: .73rem; text-overflow: ellipsis; text-transform: capitalize; white-space: nowrap; }
    .user-copy span { display: block; max-width: 135px; margin-top: 2px; overflow: hidden; color: #77869d; font-size: .61rem; text-overflow: ellipsis; white-space: nowrap; }
    .logout { display: grid; place-items: center; flex: 0 0 auto; width: 34px; height: 34px; background: #1b273c; color: #a8b3c4; border: 1px solid #29364b; border-radius: 9px; }
    .logout:hover { background: #24334b; color: #fff; }
    .app-content { min-height: 100vh; margin-left: 258px; }
    .app-topbar {
      position: sticky;
      z-index: 900;
      top: 0;
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 70px;
      padding: 0 32px;
      background: rgba(255,255,255,.9);
      border-bottom: 1px solid #e7eaf0;
      backdrop-filter: blur(14px);
    }
    .topbar-left, .top-actions, .top-user { display: flex; align-items: center; }
    .topbar-left { gap: 12px; }
    .page-context span { display: block; margin-bottom: 1px; color: #8b95a7; font-size: .62rem; font-weight: 600; letter-spacing: .055em; text-transform: uppercase; }
    .page-context b { display: block; font-family: Manrope, sans-serif; font-size: .91rem; letter-spacing: -.01em; }
    .menu-btn { display: none; place-items: center; width: 38px; height: 38px; padding: 0; background: #f4f6f9; color: #3a465a; border: 1px solid #e4e7ec; border-radius: 9px; }
    .top-actions { gap: 10px; }
    .date-chip { display: flex; align-items: center; gap: 7px; height: 36px; padding: 0 11px; color: #667085; border: 1px solid #e4e7ec; border-radius: 9px; font-size: .7rem; font-weight: 600; }
    .top-icon-button { position: relative; display: grid; place-items: center; width: 36px; height: 36px; color: #536075; border: 1px solid #e4e7ec; border-radius: 9px; }
    .top-icon-button:hover { background: #f8fafc; color: #2855d9; }
    .top-icon-button i { position: absolute; top: -5px; right: -5px; display: grid; place-items: center; min-width: 18px; height: 18px; padding: 0 4px; background: #e5484d; color: #fff; border: 2px solid #fff; border-radius: 10px; font-size: .54rem; font-style: normal; font-weight: 700; }
    .top-user { gap: 9px; margin-left: 2px; padding-left: 10px; border-left: 1px solid #e7eaf0; }
    .top-user strong { display: block; max-width: 110px; overflow: hidden; color: #273348; font-size: .71rem; text-overflow: ellipsis; text-transform: capitalize; white-space: nowrap; }
    .top-user span { display: block; margin-top: 2px; color: #8b95a7; font-size: .61rem; }
    .nav-progress { position: fixed; z-index: 1200; top: 0; right: 0; left: 258px; height: 3px; overflow: hidden; background: rgba(40,85,217,.1); }
    .nav-progress span { display: block; width: 35%; height: 100%; background: #3566e3; animation: navload .9s ease-in-out infinite; }
    @keyframes navload { from { transform: translateX(-110%); } to { transform: translateX(350%); } }
    .mobile-overlay { display: none; }

    @media (max-width: 900px) {
      .app-sidebar { transform: translateX(-102%); transition: transform .22s ease; }
      .app-sidebar.open { transform: translateX(0); box-shadow: 20px 0 60px rgba(16,24,40,.2); }
      .sidebar-close { display: grid; place-items: center; }
      .app-content { margin-left: 0; }
      .app-topbar { padding: 0 18px; }
      .menu-btn { display: grid; }
      .nav-progress { left: 0; }
      .mobile-overlay { position: fixed; z-index: 950; inset: 0; display: block; width: 100%; height: 100%; padding: 0; background: rgba(16,24,40,.48); border: 0; backdrop-filter: blur(2px); }
    }
    @media (max-width: 620px) {
      .app-topbar { height: 64px; padding: 0 14px; }
      .date-chip, .top-user > div:last-child { display: none; }
      .top-user { padding-left: 8px; }
      .page-context span { display: none; }
    }
  `]
})
export class AppShellComponent implements OnInit {
  menuOpen = false;
  navigating = false;
  pageTitle = 'Dashboard';
  today = new Intl.DateTimeFormat('en', { weekday: 'short', month: 'short', day: 'numeric' }).format(new Date());

  readonly studentMenu: NavigationItem[] = [
    { path: '/dashboard', label: 'Dashboard', icon: 'dashboard' },
    { path: '/profile', label: 'My Profile', icon: 'user' },
    { path: '/hostels', label: 'Hostel', icon: 'building' },
    { path: '/labs', label: 'Lab Reservations', icon: 'lab' },
    { path: '/events', label: 'Events', icon: 'calendar' },
    { path: '/complaints', label: 'Complaints', icon: 'message' },
    { path: '/certificates', label: 'Certificates', icon: 'certificate' },
    { path: '/fees', label: 'Fees & Receipts', icon: 'wallet' },
    { path: '/notifications', label: 'Notifications', icon: 'bell' }
  ];
  readonly adminMenu: NavigationItem[] = [
    { path: '/dashboard', label: 'Dashboard', icon: 'dashboard' },
    { path: '/admin/students', label: 'Students', icon: 'users' },
    { path: '/admin/hostels', label: 'Hostels', icon: 'building' },
    { path: '/admin/labs', label: 'Labs', icon: 'lab' },
    { path: '/admin/events', label: 'Events', icon: 'calendar' },
    { path: '/admin/complaints', label: 'Complaints', icon: 'message' },
    { path: '/admin/certificates', label: 'Certificates', icon: 'certificate' },
    { path: '/admin/fees', label: 'Fees', icon: 'wallet' },
    { path: '/admin/settings', label: 'System Settings', icon: 'settings' }
  ];

  get menu(): NavigationItem[] { return this.auth.role() === 'Admin' ? this.adminMenu : this.studentMenu; }
  get initial(): string { return (this.auth.user().email?.[0] || 'U').toUpperCase(); }
  get displayName(): string {
    const name = this.auth.user().email?.split('@')[0]?.replace(/[._-]+/g, ' ') || 'Portal user';
    return name;
  }

  constructor(
    public auth: AuthService,
    public notifications: NotificationService,
    private router: Router
  ) {
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) this.navigating = true;
      if (event instanceof NavigationEnd || event instanceof NavigationCancel || event instanceof NavigationError) this.navigating = false;
    });
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => {
      this.updatePageTitle();
      if (this.auth.role() === 'Student') this.notifications.refreshUnread();
    });
  }

  ngOnInit(): void {
    this.updatePageTitle();
    if (this.auth.role() === 'Student') this.notifications.refreshUnread();
  }

  private updatePageTitle(): void {
    const path = this.router.url.split('?')[0];
    this.pageTitle = this.menu.find(item => path.startsWith(item.path) && item.path !== '/dashboard')?.label
      ?? (path.includes('receipt') ? 'Payment Receipt' : 'Dashboard');
  }

  @HostListener('window:resize')
  onResize(): void { if (window.innerWidth > 900) this.menuOpen = false; }

  @HostListener('document:keydown.escape')
  closeMenu(): void { this.menuOpen = false; }
}
