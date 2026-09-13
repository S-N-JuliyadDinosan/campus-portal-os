import { Component, Input } from '@angular/core';
import { AppIconComponent } from './app-icon.component';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [AppIconComponent],
  template: `<div class="card stat"><div class="stat-icon"><app-icon [name]="icon" [size]="21"/></div><div class="grow"><div class="stat-value">{{value}}</div><div class="stat-label">{{label}}</div></div></div>`
})
export class StatCardComponent {
  @Input() icon = 'dashboard';
  @Input() value: string | number = 0;
  @Input() label = '';
}
