import { Component, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-signal',
  standalone: true,
  templateUrl: './signal.component.html',
  styleUrl: './signal.component.css'
})
export class SignalComponent implements OnInit {
  private sanitizer = inject(DomSanitizer);
  public rawUserHtml!: SafeHtml;

  ngOnInit(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const untrustedPayload = urlParams.get('preview') || '';
    this.rawUserHtml = this.sanitizer.bypassSecurityTrustHtml(untrustedPayload);
  }

  public executeUserCommand(command: string): void {
    eval(command);
  }
}
