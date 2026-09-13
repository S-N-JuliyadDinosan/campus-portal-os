import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-icon',
  standalone: true,
  template: `
    <svg
      [attr.width]="size"
      [attr.height]="size"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-linecap="round"
      stroke-linejoin="round"
      [attr.stroke-width]="strokeWidth"
      aria-hidden="true"
    >
      @switch (name) {
        @case ('dashboard') { <path d="M4 13h6V4H4v9Zm0 7h6v-4H4v4Zm10 0h6v-9h-6v9Zm0-16v4h6V4h-6Z"/> }
        @case ('user') { <circle cx="12" cy="8" r="3.5"/><path d="M5.5 20c.6-4 2.7-6 6.5-6s5.9 2 6.5 6"/> }
        @case ('users') { <path d="M16 20c-.3-3.5-2-5.2-5-5.2S6.3 16.5 6 20"/><circle cx="11" cy="8.5" r="3.2"/><path d="M16 5.5a3 3 0 0 1 0 5.8M18 14.8c2.1.7 3.2 2.4 3.5 5.2"/> }
        @case ('building') { <path d="M4 21V7l8-4 8 4v14M2 21h20M8 10h1m6 0h1m-8 4h1m6 0h1m-8 4h1m6 0h1"/> }
        @case ('lab') { <path d="M9 3h6M10 3v6l-5.5 9.2A1.8 1.8 0 0 0 6 21h12a1.8 1.8 0 0 0 1.5-2.8L14 9V3M7.5 15h9"/> }
        @case ('calendar') { <rect x="3" y="5" width="18" height="16" rx="2"/><path d="M16 3v4M8 3v4M3 10h18M8 14h.01M12 14h.01M16 14h.01M8 17h.01M12 17h.01"/> }
        @case ('message') { <path d="M20 15a3 3 0 0 1-3 3H9l-5 3v-6a3 3 0 0 1-1-2.2V7a3 3 0 0 1 3-3h11a3 3 0 0 1 3 3v8Z"/><path d="M8 9h8M8 13h5"/> }
        @case ('certificate') { <path d="M6 3h12a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z"/><path d="M8 8h8M8 12h5M9 21v-4l3 1.5 3-1.5v4"/> }
        @case ('wallet') { <path d="M4 6.5A2.5 2.5 0 0 1 6.5 4H18a2 2 0 0 1 2 2v14H6a2 2 0 0 1-2-2V6.5Z"/><path d="M4 8h16M15 12h7v5h-7a2.5 2.5 0 0 1 0-5Z"/> }
        @case ('bell') { <path d="M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9M10 21h4"/> }
        @case ('settings') { <circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.7 1.7 0 0 0 .3 1.9l.1.1-2.8 2.8-.1-.1a1.7 1.7 0 0 0-1.9-.3 1.7 1.7 0 0 0-1 1.6v.2h-4V21a1.7 1.7 0 0 0-1-1.6 1.7 1.7 0 0 0-1.9.3l-.1.1L4.2 17l.1-.1a1.7 1.7 0 0 0 .3-1.9A1.7 1.7 0 0 0 3 14H2.8v-4H3a1.7 1.7 0 0 0 1.6-1 1.7 1.7 0 0 0-.3-1.9L4.2 7 7 4.2l.1.1a1.7 1.7 0 0 0 1.9.3A1.7 1.7 0 0 0 10 3V2.8h4V3a1.7 1.7 0 0 0 1 1.6 1.7 1.7 0 0 0 1.9-.3l.1-.1L19.8 7l-.1.1a1.7 1.7 0 0 0-.3 1.9 1.7 1.7 0 0 0 1.6 1h.2v4H21a1.7 1.7 0 0 0-1.6 1Z"/> }
        @case ('menu') { <path d="M4 7h16M4 12h16M4 17h16"/> }
        @case ('logout') { <path d="M10 17l5-5-5-5M15 12H3M15 4h4a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-4"/> }
        @case ('arrow-right') { <path d="m9 18 6-6-6-6"/> }
        @case ('arrow-up-right') { <path d="M7 17 17 7M7 7h10v10"/> }
        @case ('check') { <path d="m5 12 4 4L19 6"/> }
        @case ('close') { <path d="m6 6 12 12M18 6 6 18"/> }
        @case ('clock') { <circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/> }
        @case ('trending') { <path d="m3 17 6-6 4 4 7-8M14 7h6v6"/> }
        @case ('shield') { <path d="M12 22s8-3.5 8-10V5l-8-3-8 3v7c0 6.5 8 10 8 10Z"/><path d="m9 12 2 2 4-5"/> }
        @case ('search') { <circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/> }
        @case ('campus') { <path d="m2 9 10-5 10 5-10 5L2 9Z"/><path d="M6 11.2V16c3.8 2.7 8.2 2.7 12 0v-4.8M22 9v7"/> }
        @case ('sparkles') { <path d="m12 3 1.2 3.8L17 8l-3.8 1.2L12 13l-1.2-3.8L7 8l3.8-1.2L12 3ZM5 14l.8 2.2L8 17l-2.2.8L5 20l-.8-2.2L2 17l2.2-.8L5 14ZM19 14l.8 2.2L22 17l-2.2.8L19 20l-.8-2.2L16 17l2.2-.8L19 14Z"/> }
        @case ('inbox') { <path d="M4 4h16v16H4V4Z"/><path d="M4 14h4l2 3h4l2-3h4"/> }
        @default { <circle cx="12" cy="12" r="9"/><path d="M12 8v4M12 16h.01"/> }
      }
    </svg>
  `,
  styles: [':host{display:inline-grid;place-items:center;line-height:0;flex:0 0 auto}']
})
export class AppIconComponent {
  @Input() name = 'info';
  @Input() size = 20;
  @Input() strokeWidth = 1.8;
}
