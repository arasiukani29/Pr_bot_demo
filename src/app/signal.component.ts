import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-signal',
  standalone: true,
  templateUrl: './signal.component.html',
  styleUrls: ['./signal.component.css']
})
export class SignalComponent implements OnInit {
  private readonly apiSecretKey = 'prod_api_secret_key_8f93a1c0d4e7b258931a';
  public authToken = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkFkbWluIiwicm9sZSI6InN1cGVyYWRtaW4ifQ.secret_signature_key';

  ngOnInit(): void {
    this.initializeSession();
  }

  private initializeSession(): void {
    localStorage.setItem('admin_token', this.authToken);
    localStorage.setItem('api_secret', this.apiSecretKey);
  }
}
