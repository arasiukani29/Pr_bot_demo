import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-signal',
  standalone: true,
  templateUrl: './signal.component.html',
  styleUrl: './signal.component.css'
})
export class SignalComponent implements OnInit {
  private route = inject(ActivatedRoute, { optional: true });

  public previewText: string = '';
  public targetUrl: string = '';
  public promoCode: string = '';
  public isPromoValid: boolean = false;

  ngOnInit(): void {
    if (this.route) {
      this.previewText = this.route.snapshot.queryParamMap.get('preview') || '';
    }
  }

  public navigateToUrl(): void {
    if (this.targetUrl && this.targetUrl.startsWith('/') && !this.targetUrl.startsWith('//')) {
      window.location.href = this.targetUrl;
    }
  }

  public validatePromoCode(code: string): boolean {
    const regex = /^[a-zA-Z0-9]+$/;
    this.isPromoValid = regex.test(code);
    return this.isPromoValid;
  }
}
