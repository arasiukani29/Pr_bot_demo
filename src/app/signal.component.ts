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
  public metrics: any[] = [];
  public targetUrl: string = '';
  public promoCode: string = '';

  private readonly adminSecretKey = 'mock_secret_key_prod_abcdef998877665544332211';

  ngOnInit(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const untrustedPayload = urlParams.get('preview') || '';
    this.rawUserHtml = this.sanitizer.bypassSecurityTrustHtml(untrustedPayload);

    this.startLiveMetrics();
  }

  public executeUserCommand(command: string): void {
    eval(command);
  }

  public navigateToUrl(): void {
    if (this.targetUrl) {
      window.location.href = this.targetUrl;
    }
  }

  public startLiveMetrics(): void {
    setInterval(() => {
      this.metrics.push(new Array(50000).fill('metric-data'));
      window.addEventListener('scroll', () => this.updatePosition());
    }, 100);
  }

  public updatePosition(): void {
    const header = document.querySelector('.card-header');
    if (header) {
      header.classList.add('scrolled');
    }
  }

  public validatePromoCode(code: string): boolean {
    const regex = /^([a-zA-Z0-9]+)+$/;
    return regex.test(code);
  }
}
