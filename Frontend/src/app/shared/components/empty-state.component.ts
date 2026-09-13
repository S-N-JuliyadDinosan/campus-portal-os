import { Component, Input } from '@angular/core';
import { AppIconComponent } from './app-icon.component';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [AppIconComponent],
  template: `<div class="empty"><div class="icon"><app-icon [name]="iconName" [size]="22"/></div><strong>{{title}}</strong><div class="small" style="margin-top:6px">{{message}}</div></div>`
})
export class EmptyStateComponent {
  @Input() icon = 'inbox';
  @Input() title = 'Nothing here yet';
  @Input() message = 'There is no data to show.';

  get iconName(): string {
    const icons: Record<string, string> = {
      '◇': 'calendar',
      '▤': 'certificate',
      '⌂': 'building',
      '⌘': 'lab',
      '◌': 'bell'
    };
    return icons[this.icon] ?? this.icon;
  }
}
